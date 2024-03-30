using System;

namespace Massive
{
	public class MassiveGroupsController : GroupsController, IMassive
	{
		private readonly int _framesCapacity;
		private int _savedFrames;

		public MassiveGroupsController(int nonOwningDataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
			: base(nonOwningDataCapacity)
		{
			_framesCapacity = framesCapacity;
		}

		public int CanRollbackFrames => _savedFrames;

		public void SaveFrame()
		{
			_savedFrames += Math.Min(_savedFrames + 1, _framesCapacity);

			// ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
			foreach (IMassive massive in CreatedGroups)
			{
				massive.SaveFrame();
			}
		}

		public void Rollback(int frames)
		{
			_savedFrames -= frames;

			// ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
			foreach (IMassive massive in CreatedGroups)
			{
				massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
			}
		}

		protected override IGroup CreateOwningGroup(ISet[] owned, IReadOnlySet[] include = null, IReadOnlySet[] exclude = null)
		{
			var massiveOwningGroup = new MassiveOwningGroup(owned, include, exclude, _framesCapacity);
			massiveOwningGroup.SaveFrame(); // Save first empty frame so we can rollback to it
			return massiveOwningGroup;
		}

		protected override IGroup CreateNonOwningGroup(IReadOnlySet[] include, IReadOnlySet[] exclude = null, int dataCapacity = Constants.DataCapacity)
		{
			var massiveNonOwningGroup = new MassiveNonOwningGroup(include, exclude, dataCapacity, _framesCapacity);
			massiveNonOwningGroup.SaveFrame(); // Save first empty frame so we can rollback to it
			return massiveNonOwningGroup;
		}
	}
}