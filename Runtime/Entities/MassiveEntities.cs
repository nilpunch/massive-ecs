using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="Massive.Entities"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class MassiveEntities : Entities, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly int[][] _packedByFrames;
		private readonly uint[][] _versionsByFrames;
		private readonly int[][] _sparseByFrames;
		private readonly State[] _stateByFrames;

		public MassiveEntities(int framesCapacity = Constants.DefaultFramesCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_packedByFrames = new int[framesCapacity][];
			_versionsByFrames = new uint[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_stateByFrames = new State[framesCapacity];

			for (var i = 0; i < framesCapacity; i++)
			{
				_packedByFrames[i] = Array.Empty<int>();
				_versionsByFrames[i] = Array.Empty<uint>();
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
			Array.Copy(Packed, _packedByFrames[currentFrame], MaxId);
			Array.Copy(Versions, _versionsByFrames[currentFrame], MaxId);
			Array.Copy(Sparse, _sparseByFrames[currentFrame], MaxId);
			_stateByFrames[currentFrame] = CurrentState;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			var rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			var rollbackState = _stateByFrames[rollbackFrame];

			Array.Copy(_packedByFrames[rollbackFrame], Packed, rollbackState.MaxId);
			Array.Copy(_versionsByFrames[rollbackFrame], Versions, rollbackState.MaxId);
			Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackState.MaxId);
			CurrentState = rollbackState;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityForFrame(int frame)
		{
			if (_packedByFrames[frame].Length < PackedCapacity)
			{
				Array.Resize(ref _packedByFrames[frame], PackedCapacity);
				Array.Resize(ref _versionsByFrames[frame], PackedCapacity);
			}
			if (_sparseByFrames[frame].Length < SparseCapacity)
			{
				Array.Resize(ref _sparseByFrames[frame], SparseCapacity);
			}
		}
	}
}
