using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.MassiveSparseSet"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class MassiveDataSet<T> : DataSet<T>, IMassiveSet where T : unmanaged
	{
		private readonly int[] _denseByFrames;
		private readonly int[] _sparseByFrames;
		private readonly int[] _maxDenseByFrames;
		private readonly int[] _maxIdByFrames;
		private readonly int[] _aliveCountByFrames;

		private readonly int _framesCapacity;
		private int _currentFrame;
		private int _savedFrames;
		
		private readonly T[] _dataByFrames;

		public MassiveDataSet(int framesCapacity = Constants.FramesCapacity, int dataCapacity = Constants.DataCapacity)
			: base(dataCapacity)
		{
			_framesCapacity = framesCapacity;

			_dataByFrames = new T[framesCapacity * Capacity];
			_denseByFrames = new int[framesCapacity * Capacity];
			_sparseByFrames = new int[framesCapacity * Capacity];
			_maxDenseByFrames = new int[framesCapacity];
			_maxIdByFrames = new int[framesCapacity];
			_aliveCountByFrames = new int[framesCapacity];
		}

		/// <summary>
		/// Can be negative, when there absolutely no saved frames to restore information.
		/// </summary>
		public int CanRollbackFrames => _savedFrames - 1;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			int currentMaxDense = Storage.SparseSet.MaxDense;
			int currentMaxId = Storage.SparseSet.MaxId;
			int currentAliveCount = AliveCount;
			int nextFrame = Loop(_currentFrame + 1, _framesCapacity);

			// Copy everything from current state to next frame
			Array.Copy(Storage.Data, 0, _dataByFrames, nextFrame * Capacity, currentAliveCount);
			Array.Copy(Storage.SparseSet.Dense, 0, _denseByFrames, nextFrame * Capacity, currentMaxDense);
			Array.Copy(Storage.SparseSet.Sparse, 0, _sparseByFrames, nextFrame * Capacity, currentMaxId);
			_maxDenseByFrames[nextFrame] = currentMaxDense;
			_maxIdByFrames[nextFrame] = currentMaxId;
			_aliveCountByFrames[nextFrame] = currentAliveCount;

			_currentFrame = nextFrame;
			_savedFrames = Math.Min(_savedFrames + 1, _framesCapacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			if (frames > CanRollbackFrames)
			{
				throw new InvalidOperationException($"Can't rollback this far. CanRollback:{CanRollbackFrames}, Requested: {frames}.");
			}

			_savedFrames -= frames;
			_currentFrame = LoopNegative(_currentFrame - frames, _framesCapacity);

			// Copy everything from rollback frame to current state
			int rollbackMaxDense = _maxDenseByFrames[_currentFrame];
			int rollbackMaxId = _maxIdByFrames[_currentFrame];
			int rollbackAliveCount = _aliveCountByFrames[_currentFrame];
			int rollbackFrame = _currentFrame;

			// Copy rollback data
			Array.Copy(_dataByFrames, rollbackFrame * Capacity, Storage.Data, 0, rollbackAliveCount);
			
			// Copy last MaxDense and MaxId elements to ensure zeroing excess
			Array.Copy(_denseByFrames, rollbackFrame * Capacity, Storage.SparseSet.Dense, 0, Storage.SparseSet.MaxDense);
			Array.Copy(_sparseByFrames, rollbackFrame * Capacity, Storage.SparseSet.Sparse, 0, Storage.SparseSet.MaxId);
			Storage.SparseSet.MaxDense = rollbackMaxDense;
			Storage.SparseSet.MaxId = rollbackMaxId;
			Storage.SparseSet.AliveCount = rollbackAliveCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Loop(int a, int b)
		{
			return a % b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LoopNegative(int a, int b)
		{
			int result = a % b;

			if (result < 0)
			{
				return result + b;
			}

			return result;
		}
	}
}