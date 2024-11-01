using System.Collections.Generic;

namespace Massive
{
	public class MassiveGroupFactory : IGroupFactory
	{
		private readonly int _framesCapacity;

		public MassiveGroupFactory(int framesCapacity = Constants.DefaultFramesCapacity)
		{
			_framesCapacity = framesCapacity;
		}

		public OwningGroup CreateOwningGroup(IReadOnlyList<SparseSet> owned, IReadOnlyList<SparseSet> include = null, IReadOnlyList<SparseSet> exclude = null)
		{
			var massiveOwningGroup = new MassiveOwningGroup(owned, include, exclude, _framesCapacity);
			massiveOwningGroup.SaveFrame(); // Save first empty frame so we can rollback to it
			return massiveOwningGroup;
		}

		public NonOwningGroup CreateNonOwningGroup(IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude = null, Entities entities = null)
		{
			var massiveNonOwningGroup = new MassiveNonOwningGroup(include, exclude, _framesCapacity, entities);
			massiveNonOwningGroup.SaveFrame(); // Save first empty frame so we can rollback to it
			return massiveNonOwningGroup;
		}
	}
}
