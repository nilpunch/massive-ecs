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
		private readonly struct Info
		{
			public readonly int Count;
			public readonly int MaxId;
			public readonly int NextHoleId;
			public readonly PackingMode PackingMode;

			public Info(int count, int maxId, int nextHoleId, PackingMode packingMode)
			{
				Count = count;
				MaxId = maxId;
				NextHoleId = nextHoleId;
				PackingMode = packingMode;
			}
		}

		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly int[][] _idsByFrames;
		private readonly uint[][] _versionsByFrames;
		private readonly int[][] _sparseByFrames;
		private readonly Info[] _infoByFrames;

		public MassiveEntities(int framesCapacity = Constants.DefaultFramesCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_idsByFrames = new int[framesCapacity][];
			_versionsByFrames = new uint[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_infoByFrames = new Info[framesCapacity];

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
			Info currentInfo = new Info(Count, MaxId, NextHoleId, PackingMode);

			EnsureCapacityForFrame(currentFrame);

			// Copy everything from current state to current frame
			Array.Copy(Ids, _idsByFrames[currentFrame], currentInfo.MaxId);
			Array.Copy(Versions, _versionsByFrames[currentFrame], currentInfo.MaxId);
			Array.Copy(Sparse, _sparseByFrames[currentFrame], currentInfo.MaxId);
			_infoByFrames[currentFrame] = currentInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			Info rollbackInfo = _infoByFrames[rollbackFrame];

			Array.Copy(_idsByFrames[rollbackFrame], Ids, rollbackInfo.MaxId);
			Array.Copy(_versionsByFrames[rollbackFrame], Versions, rollbackInfo.MaxId);
			Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackInfo.MaxId);
			Count = rollbackInfo.Count;
			MaxId = rollbackInfo.MaxId;
			NextHoleId = rollbackInfo.NextHoleId;
			PackingMode = rollbackInfo.PackingMode;
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
