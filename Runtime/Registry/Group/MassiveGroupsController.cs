using System;

namespace Massive
{
	public class MassiveGroupsController : GroupsController, IMassive
	{
		private readonly int _framesCapacity;
		private int _currentFrame;

		public MassiveGroupsController(int nonOwningDataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
			: base(nonOwningDataCapacity)
		{
			_framesCapacity = framesCapacity;
		}

		public int CanRollbackFrames => _currentFrame;

		public void SaveFrame()
		{
			_currentFrame += Math.Min(_currentFrame + 1, _framesCapacity);

			// ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
			foreach (IMassive massive in CreatedGroups)
			{
				massive.SaveFrame();
			}
		}

		public void Rollback(int frames)
		{
			_currentFrame -= frames;

			// ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
			foreach (IMassive massive in CreatedGroups)
			{
				massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
			}
		}

		protected override IGroup CreateOwningGroup(ISet[] owned, ISet[] other = null, IFilter filter = null)
		{
			return new MassiveOwningGroup(owned, other, filter, _framesCapacity);
		}

		protected override IGroup CreateNonOwningGroup(ISet[] other, IFilter filter = null, int dataCapacity = Constants.DataCapacity)
		{
			return new MassiveNonOwningGroup(other, filter, dataCapacity, _framesCapacity);
		}
	}
}