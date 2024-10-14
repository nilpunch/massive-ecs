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
			int pageSize = Constants.DefaultPageSize) : base(setCapacity, pageSize)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_dataByFrames = new PagedArray<T>[framesCapacity];
			_packedByFrames = new int[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_countByFrames = new int[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
			{
				_dataByFrames[i] = new PagedArray<T>(pageSize);
				_packedByFrames[i] = new int[PackedCapacity];
				_sparseByFrames[i] = new int[SparseCapacity];
			}
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			int currentFrame = _cyclicFrameCounter.CurrentFrame;
			int currentCount = Count;

			// Copy everything from current state to current frame
			CopyData(Data, _dataByFrames[currentFrame], currentCount);
			Array.Copy(Packed, _packedByFrames[currentFrame], currentCount);
			Array.Copy(Sparse, _sparseByFrames[currentFrame], SparseCapacity);
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
			Array.Copy(_packedByFrames[rollbackFrame], Packed, rollbackCount);
			Array.Copy(_sparseByFrames[rollbackFrame], Sparse, SparseCapacity);
			Count = rollbackCount;
		}

		protected abstract void CopyData(PagedArray<T> source, PagedArray<T> destination, int count);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void ResizePacked(int capacity)
		{
			base.ResizePacked(capacity);

			for (int i = 0; i < _cyclicFrameCounter.FramesCapacity; i++)
			{
				Array.Resize(ref _packedByFrames[i], capacity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void ResizeSparse(int capacity)
		{
			int previousCapacity = SparseCapacity;
			base.ResizeSparse(capacity);

			for (int i = 0; i < _cyclicFrameCounter.FramesCapacity; i++)
			{
				Array.Resize(ref _sparseByFrames[i], capacity);
				if (capacity > previousCapacity)
				{
					Array.Fill(_sparseByFrames[i], Constants.InvalidId, previousCapacity, capacity - previousCapacity);
				}
			}
		}
	}
}
