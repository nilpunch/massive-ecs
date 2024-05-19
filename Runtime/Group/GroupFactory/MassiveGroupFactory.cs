using System.Collections.Generic;

namespace Massive
{
	public class MassiveGroupFactory : IGroupFactory
	{
		private readonly int _nonOwningSetCapacity;
		private readonly int _framesCapacity;

		public MassiveGroupFactory(int nonOwningSetCapacity = Constants.DefaultSetCapacity, int framesCapacity = Constants.DefaultFramesCapacity)
		{
			_nonOwningSetCapacity = nonOwningSetCapacity;
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
			var massiveNonOwningGroup = new MassiveNonOwningGroup(include, exclude, _nonOwningSetCapacity, _framesCapacity);
			massiveNonOwningGroup.SaveFrame(); // Save first empty frame so we can rollback to it
			return massiveNonOwningGroup;
		}
	}
}
