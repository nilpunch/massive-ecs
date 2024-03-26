using System;

namespace Massive
{
	public class MassiveNonOwningGroup : NonOwningGroup, IMassive
	{
		private readonly int _framesCapacity;
		private int _currentFrame;

		public MassiveNonOwningGroup(IRegistry registry, ISet[] other, int framesCapacity = Constants.FramesCapacity) : base(registry, other)
		{
			_framesCapacity = framesCapacity;
		}

		public int CanRollbackFrames => _currentFrame;

		public void SaveFrame()
		{
			// If _currentFrame == _framesCapacity + 1,
			// Then this is group is safe forever
			_currentFrame = Math.Min(_currentFrame + 1, _framesCapacity + 1);
		}

		public void Rollback(int frames)
		{
			_currentFrame -= frames;

			if (_currentFrame <= 0)
			{
				IsWaken = false;
			}
		}
	}
}