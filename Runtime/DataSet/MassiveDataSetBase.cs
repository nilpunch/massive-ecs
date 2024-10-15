using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.MassiveSparseSet"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public abstract class MassiveDataSetBase<T> : DataSet<T>, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly PagedArray<T>[] _dataByFrames;
		private readonly int[][] _packedByFrames;
		private readonly int[][] _sparseByFrames;
		private readonly int[] _countByFrames;

		protected MassiveDataSetBase(int setCapacity = Constants.DefaultCapacity, int framesCapacity = Constants.DefaultFramesCapacity,
			int pageSize = Constants.DefaultPageSize, IndexingMode indexingMode = IndexingMode.Packed) : base(setCapacity, pageSize, indexingMode)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_dataByFrames = new PagedArray<T>[framesCapacity];
			_packedByFrames = new int[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_countByFrames = new int[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
			{
				_dataByFrames[i] = new PagedArray<T>(pageSize);
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
			int currentCount = Count;

			EnsureCapacityForFrame(currentFrame);

			// Copy everything from current state to current frame
			CopyData(Data, _dataByFrames[currentFrame], currentCount);
			if (IsPacked)
			{
				Array.Copy(Packed, _packedByFrames[currentFrame], currentCount);
				Array.Copy(Sparse, _sparseByFrames[currentFrame], SparseCapacity);
			}
			else
			{
				Array.Copy(Sparse, _sparseByFrames[currentFrame], currentCount);
			}
			_countByFrames[currentFrame] = currentCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			int rollbackCount = _countByFrames[rollbackFrame];

			CopyData(_dataByFrames[rollbackFrame], Data, rollbackCount);
			if (IsPacked)
			{
				int rollbackSparseCapacity = _sparseByFrames[rollbackFrame].Length;
				Array.Copy(_packedByFrames[rollbackFrame], Packed, rollbackCount);
				Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackSparseCapacity);
				if (rollbackSparseCapacity < SparseCapacity)
				{
					Array.Fill(Sparse, Constants.InvalidId, rollbackSparseCapacity, SparseCapacity - rollbackSparseCapacity);
				}
			}
			else
			{
				Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackCount);
				if (rollbackCount < Count)
				{
					Array.Fill(Sparse, Constants.InvalidId, rollbackCount, Count - rollbackCount);
				}
			}
			Count = rollbackCount;
		}

		protected abstract void CopyData(PagedArray<T> source, PagedArray<T> destination, int count);

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
