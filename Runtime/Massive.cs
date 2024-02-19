using System;
using System.Runtime.CompilerServices;

namespace MassiveData
{
	/// <summary>
	/// Data wrapper around <see cref="MassiveData.MassiveSparseSet"/>.
	/// </summary>
	/// <remarks>
	/// Has index shift to compensate for reserved dense element in <see cref="MassiveData.MassiveSparseSet"/>,
	/// so that alive dense indices starts from zero and not from one.
	/// </remarks>
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public class Massive<T> : IMassive where T : struct
	{
		private readonly MassiveSparseSet _sparseSet;

		// Saved frames
		private readonly T[] _dataByFrames;

		// Current frame
		private readonly T[] _currentData;

		public Massive(int framesCapacity = 120, int dataCapacity = 100)
		{
			// Compensate for reserved rollback frame
			framesCapacity += 1;
			
			_sparseSet = new MassiveSparseSet(framesCapacity, dataCapacity);
			_dataByFrames = new T[framesCapacity * dataCapacity];
			_currentData = new T[dataCapacity];
		}

		public Span<T> AliveData => new Span<T>(_currentData, 0, _sparseSet.AliveCount - 1);
		
		public int AliveCount => _sparseSet.AliveCount - 1;

		public int CanRollbackFrames => _sparseSet.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			var saveInfo = _sparseSet.SaveFrame();
			Array.Copy(_currentData, 0, _dataByFrames, saveInfo.NextFrame * _currentData.Length, saveInfo.DenseCount - 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			var rollbackInfo = _sparseSet.Rollback(frames);
			Array.Copy(_dataByFrames, rollbackInfo.RollbackFrame * _currentData.Length, _currentData, 0, rollbackInfo.DenseCount - 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Create(T data = default)
		{
			var createInfo = _sparseSet.Create();
			_currentData[createInfo.Dense - 1] = data;
			return createInfo.Id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Delete(int id)
		{
			var deleteInfo = _sparseSet.Delete(id);
			if (deleteInfo.HasValue)
			{
				_currentData[deleteInfo.Value.DenseSwapTarget - 1] = _currentData[deleteInfo.Value.DenseSwapSource - 1];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DeleteDense(int denseIndex)
		{
			var deleteInfo = _sparseSet.DeleteDense(denseIndex + 1);
			if (deleteInfo.HasValue)
			{
				_currentData[deleteInfo.Value.DenseSwapTarget - 1] = _currentData[deleteInfo.Value.DenseSwapSource - 1];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			return ref _currentData[_sparseSet.GetDenseIndex(id) - 1];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			return _sparseSet.IsAlive(id);
		}
	}
}