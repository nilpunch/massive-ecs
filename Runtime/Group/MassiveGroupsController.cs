using System;
using System.Collections.Generic;

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
			foreach (IMassive massive in AllGroups)
			{
				massive.SaveFrame();
			}
		}

		public void Rollback(int frames)
		{
			_savedFrames -= frames;

			// ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
			foreach (IMassive massive in AllGroups)
			{
				massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
			}
		}

		protected override IOwningGroup CreateOwningGroup(IReadOnlyList<ISet> owned,
			IReadOnlyList<IReadOnlySet> include = null, IReadOnlyList<IReadOnlySet> exclude = null)
		{
			var massiveOwningGroup = new MassiveOwningGroup(owned, include, exclude, _framesCapacity);
			massiveOwningGroup.SaveFrame(); // Save first empty frame so we can rollback to it
			return massiveOwningGroup;
		}

		protected override IGroup CreateNonOwningGroup(IReadOnlyList<IReadOnlySet> include,
			IReadOnlyList<IReadOnlySet> exclude = null, int dataCapacity = Constants.DataCapacity)
		{
			var massiveNonOwningGroup = new MassiveNonOwningGroup(include, exclude, dataCapacity, _framesCapacity);
			massiveNonOwningGroup.SaveFrame(); // Save first empty frame so we can rollback to it
			return massiveNonOwningGroup;
		}
	}
}
