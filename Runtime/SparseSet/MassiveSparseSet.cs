using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="Massive.SparseSet"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class MassiveSparseSet : SparseSet, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly int[][] _packedByFrames;
		private readonly int[][] _sparseByFrames;
		private readonly State[] _stateByFrames;

		public MassiveSparseSet(int framesCapacity = Constants.DefaultFramesCapacity, PackingMode packingMode = PackingMode.Continuous)
			: base(packingMode)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_packedByFrames = new int[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_stateByFrames = new State[framesCapacity];

			for (var i = 0; i < framesCapacity; i++)
			{
				_packedByFrames[i] = Array.Empty<int>();
				_sparseByFrames[i] = Array.Empty<int>();
			}
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			var currentFrame = _cyclicFrameCounter.CurrentFrame;

			EnsureCapacityForFrame(currentFrame);

			// Copy everything from current state to current frame
			Array.Copy(Packed, _packedByFrames[currentFrame], Count);
			Array.Copy(Sparse, _sparseByFrames[currentFrame], Sparse.Length);
			_stateByFrames[currentFrame] = CurrentState;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			var rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			var rollbackSparseLength = _sparseByFrames[rollbackFrame].Length;
			var rollbackState = _stateByFrames[rollbackFrame];

			Array.Copy(_packedByFrames[rollbackFrame], Packed, rollbackState.Count);
			Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackSparseLength);
			if (rollbackSparseLength < Sparse.Length)
			{
				Array.Fill(Sparse, Constants.InvalidId, rollbackSparseLength, Sparse.Length - rollbackSparseLength);
			}
			CurrentState = rollbackState;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityForFrame(int frame)
		{
			if (_sparseByFrames[frame].Length < Sparse.Length)
			{
				var previousLength = _sparseByFrames[frame].Length;
				Array.Resize(ref _sparseByFrames[frame], Sparse.Length);
				Array.Fill(_sparseByFrames[frame], Constants.InvalidId, previousLength, Sparse.Length - previousLength);
			}

			if (_packedByFrames[frame].Length < Packed.Length)
			{
				Array.Resize(ref _packedByFrames[frame], Packed.Length);
			}
		}
	}
}
