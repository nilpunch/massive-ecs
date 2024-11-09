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

		private readonly int[][] _idsByFrames;
		private readonly uint[][] _versionsByFrames;
		private readonly int[][] _sparseByFrames;
		private readonly State[] _stateByFrames;

		public MassiveEntities(int framesCapacity = Constants.DefaultFramesCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_idsByFrames = new int[framesCapacity][];
			_versionsByFrames = new uint[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_stateByFrames = new State[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
			{
				_idsByFrames[i] = new int[Ids.Length];
				_versionsByFrames[i] = new uint[Versions.Length];
				_sparseByFrames[i] = new int[Sparse.Length];
			}
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			int currentFrame = _cyclicFrameCounter.CurrentFrame;

			EnsureCapacityForFrame(currentFrame);

			// Copy everything from current state to current frame
			Array.Copy(Ids, _idsByFrames[currentFrame], MaxId);
			Array.Copy(Versions, _versionsByFrames[currentFrame], MaxId);
			Array.Copy(Sparse, _sparseByFrames[currentFrame], MaxId);
			_stateByFrames[currentFrame] = CurrentState;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			State rollbackState = _stateByFrames[rollbackFrame];

			Array.Copy(_idsByFrames[rollbackFrame], Ids, rollbackState.MaxId);
			Array.Copy(_versionsByFrames[rollbackFrame], Versions, rollbackState.MaxId);
			Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackState.MaxId);
			CurrentState = rollbackState;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityForFrame(int frame)
		{
			if (_idsByFrames[frame].Length < Ids.Length)
			{
				Array.Resize(ref _idsByFrames[frame], Ids.Length);
			}
			if (_versionsByFrames[frame].Length < Versions.Length)
			{
				Array.Resize(ref _versionsByFrames[frame], Versions.Length);
			}
			if (_sparseByFrames[frame].Length < Sparse.Length)
			{
				Array.Resize(ref _sparseByFrames[frame], Sparse.Length);
			}
		}
	}
}
