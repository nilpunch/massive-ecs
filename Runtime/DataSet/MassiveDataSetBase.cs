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
	public abstract class MassiveDataSetBase<T> : DataSet<T>, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly PackedArray<T>[] _dataByFrames;
		private readonly PackedArray<int>[] _denseByFrames;
		private readonly PackedArray<int>[] _sparseByFrames;
		private readonly int[] _countByFrames;

		protected MassiveDataSetBase(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
			: base(dataCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_dataByFrames = new PackedArray<T>[framesCapacity];
			_denseByFrames = new PackedArray<int>[framesCapacity];
			_sparseByFrames = new PackedArray<int>[framesCapacity];
			_countByFrames = new int[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
			{
				_dataByFrames[i] = new PackedArray<T>();
				_denseByFrames[i] = new PackedArray<int>();
				_sparseByFrames[i] = new PackedArray<int>();
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
			RawData.CopyTo(_dataByFrames[currentFrame], currentCount, CopyData);
			Dense.CopyTo(_denseByFrames[currentFrame], currentCount);
			Sparse.CopyTo(_sparseByFrames[currentFrame]);
			_countByFrames[currentFrame] = currentCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			int rollbackCount = _countByFrames[rollbackFrame];

			_dataByFrames[rollbackFrame].CopyTo(RawData, rollbackCount, CopyData);
			_denseByFrames[rollbackFrame].CopyTo(Dense, rollbackCount);
			_sparseByFrames[rollbackFrame].CopyTo(Sparse);
			Count = rollbackCount;
		}

		protected abstract void CopyData(T[] source, T[] destination, int count);
	}
}
