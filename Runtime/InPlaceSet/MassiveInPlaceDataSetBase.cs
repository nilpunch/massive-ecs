using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.MassiveInPlaceSet"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public abstract class MassiveInPlaceDataSetBase<T> : InPlaceDataSet<T>, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly PagedArray<T>[] _dataByFrames;
		private readonly bool[][] _placesByFrames;
		private readonly int[] _usedPlacesCountByFrames;

		protected MassiveInPlaceDataSetBase(int setCapacity = Constants.DefaultCapacity, int framesCapacity = Constants.DefaultFramesCapacity,
			int pageSize = Constants.DefaultPageSize) : base(setCapacity, pageSize)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_dataByFrames = new PagedArray<T>[framesCapacity];
			_placesByFrames = new bool[framesCapacity][];
			_usedPlacesCountByFrames = new int[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
			{
				_dataByFrames[i] = new PagedArray<T>(pageSize);
				_placesByFrames[i] = new bool[PlacesCapacity];
			}
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			int currentFrame = _cyclicFrameCounter.CurrentFrame;
			int currentUsedPlacesCount = UsedPlacesCount;

			// Copy everything from current state to current frame
			CopyData(Data, _dataByFrames[currentFrame], currentUsedPlacesCount);
			Array.Copy(Places, _placesByFrames[currentFrame], currentUsedPlacesCount);
			_usedPlacesCountByFrames[currentFrame] = currentUsedPlacesCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			int rollbackUsedPlacesCount = _usedPlacesCountByFrames[rollbackFrame];

			CopyData(_dataByFrames[rollbackFrame], Data, UsedPlacesCount);
			Array.Copy(_placesByFrames[rollbackFrame], Places, UsedPlacesCount);
			UsedPlacesCount = rollbackUsedPlacesCount;
		}

		protected abstract void CopyData(PagedArray<T> source, PagedArray<T> destination, int count);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void ResizePlaces(int capacity)
		{
			base.ResizePlaces(capacity);

			for (int i = 0; i < _cyclicFrameCounter.FramesCapacity; i++)
			{
				Array.Resize(ref _placesByFrames[i], capacity);
			}
		}
	}
}
