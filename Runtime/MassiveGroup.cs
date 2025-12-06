using System;
using System.Collections.Generic;

namespace Massive
{
	public class MassiveGroup : IMassive
	{
		private readonly List<IMassive> _massives;
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		public MassiveGroup(int framesCapacity)
			: this(framesCapacity, Array.Empty<IMassive>())
		{
		}

		public MassiveGroup(int framesCapacity, params IMassive[] massives)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);
			_massives = new List<IMassive>(massives);
		}

		public void Add(IMassive massive)
		{
			_massives.Add(massive);
		}

		public void Remove(IMassive massive)
		{
			_massives.Remove(massive);
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			foreach (var massive in _massives)
			{
				massive.SaveFrame();
			}
		}

		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			foreach (var massive in _massives)
			{
				massive.Rollback(frames);
			}
		}
	}
}
