using System.Collections.Generic;

namespace Massive
{
	public class MassiveGroupFactory : IGroupFactory
	{
		private readonly int _nonOwningDataCapacity;
		private readonly int _framesCapacity;

		public MassiveGroupFactory(int nonOwningDataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
		{
			_nonOwningDataCapacity = nonOwningDataCapacity;
			_framesCapacity = framesCapacity;
		}

		public IOwningGroup CreateOwningGroup(IReadOnlyList<ISet> owned, IReadOnlyList<IReadOnlySet> include = null, IReadOnlyList<IReadOnlySet> exclude = null)
		{
			var massiveOwningGroup = new MassiveOwningGroup(owned, include, exclude, _framesCapacity);
			massiveOwningGroup.SaveFrame(); // Save first empty frame so we can rollback to it
			return massiveOwningGroup;
		}

		public IGroup CreateNonOwningGroup(IReadOnlyList<IReadOnlySet> include, IReadOnlyList<IReadOnlySet> exclude = null)
		{
			var massiveNonOwningGroup = new MassiveNonOwningGroup(include, exclude, _nonOwningDataCapacity, _framesCapacity);
			massiveNonOwningGroup.SaveFrame(); // Save first empty frame so we can rollback to it
			return massiveNonOwningGroup;
		}
	}
}
