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
		private readonly struct Info
		{
			public readonly int Count;
			public readonly int NextHole;
			public readonly PackingMode PackingMode;

			public Info(int count, int nextHole, PackingMode packingMode)
			{
				Count = count;
				NextHole = nextHole;
				PackingMode = packingMode;
			}
		}

		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly int[][] _packedByFrames;
		private readonly int[][] _sparseByFrames;
		private readonly Info[] _infoByFrames;

		public MassiveSparseSet(int framesCapacity = Constants.DefaultFramesCapacity, PackingMode packingMode = PackingMode.Continuous)
			: base(packingMode)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_packedByFrames = new int[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_infoByFrames = new Info[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
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

			int currentFrame = _cyclicFrameCounter.CurrentFrame;
			Info currentInfo = new Info(Count, NextHole, PackingMode);

			EnsureCapacityForFrame(currentFrame);

			// Copy everything from current state to current frame
			Array.Copy(Packed, _packedByFrames[currentFrame], currentInfo.Count);
			Array.Copy(Sparse, _sparseByFrames[currentFrame], Sparse.Length);
			_infoByFrames[currentFrame] = currentInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			int rollbackSparseLength = _sparseByFrames[rollbackFrame].Length;
			Info rollbackInfo = _infoByFrames[rollbackFrame];

			Array.Copy(_packedByFrames[rollbackFrame], Packed, rollbackInfo.Count);
			Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackSparseLength);
			if (rollbackSparseLength < Sparse.Length)
			{
				Array.Fill(Sparse, Constants.InvalidId, rollbackSparseLength, Sparse.Length - rollbackSparseLength);
			}
			Count = rollbackInfo.Count;
			NextHole = rollbackInfo.NextHole;
			PackingMode = rollbackInfo.PackingMode;
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
