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

		public MassiveSparseSet(int framesCapacity = Constants.DefaultFramesCapacity, Packing packing = Packing.Continuous)
			: base(packing)
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
			Array.Copy(Sparse, _sparseByFrames[currentFrame], SparseCapacity);
			_stateByFrames[currentFrame] = CurrentState;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			var rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			var rollbackSparseCapacity = _sparseByFrames[rollbackFrame].Length;
			var rollbackState = _stateByFrames[rollbackFrame];

			Array.Copy(_packedByFrames[rollbackFrame], Packed, rollbackState.Count);
			Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackSparseCapacity);
			if (rollbackSparseCapacity < SparseCapacity)
			{
				Array.Fill(Sparse, Constants.InvalidId, rollbackSparseCapacity, SparseCapacity - rollbackSparseCapacity);
			}
			CurrentState = rollbackState;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityForFrame(int frame)
		{
			if (_sparseByFrames[frame].Length < SparseCapacity)
			{
				var previousCapacity = _sparseByFrames[frame].Length;
				Array.Resize(ref _sparseByFrames[frame], SparseCapacity);
				Array.Fill(_sparseByFrames[frame], Constants.InvalidId, previousCapacity, SparseCapacity - previousCapacity);
			}

			if (_packedByFrames[frame].Length < PackedCapacity)
			{
				Array.Resize(ref _packedByFrames[frame], PackedCapacity);
			}
		}
	}
}
