using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class MassiveInPlaceSet : InPlaceSet, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly bool[][] _placesByFrames;
		private readonly int[] _usedPlacesCountByFrames;

		public MassiveInPlaceSet(int setCapacity = Constants.DefaultCapacity, int framesCapacity = Constants.DefaultFramesCapacity)
			: base(setCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_placesByFrames = new bool[framesCapacity][];
			_usedPlacesCountByFrames = new int[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
			{
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

			Array.Copy(_placesByFrames[rollbackFrame], Places, UsedPlacesCount);
			UsedPlacesCount = rollbackUsedPlacesCount;
		}

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
