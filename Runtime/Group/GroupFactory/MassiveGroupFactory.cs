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

		public IOwningGroup CreateOwningGroup(IReadOnlyList<SparseSet> owned, IReadOnlyList<SparseSet> include = null, IReadOnlyList<SparseSet> exclude = null)
		{
			var massiveOwningGroup = new MassiveOwningGroup(owned, include, exclude, _framesCapacity);
			massiveOwningGroup.SaveFrame(); // Save first empty frame so we can rollback to it
			return massiveOwningGroup;
		}

		public IGroup CreateNonOwningGroup(IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude = null)
		{
			var massiveNonOwningGroup = new MassiveNonOwningGroup(include, exclude, _nonOwningSetCapacity, _framesCapacity);
			massiveNonOwningGroup.SaveFrame(); // Save first empty frame so we can rollback to it
			return massiveNonOwningGroup;
		}
	}
}
