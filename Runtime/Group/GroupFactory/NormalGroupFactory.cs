using System.Collections.Generic;

namespace Massive
{
	public class NormalGroupFactory : IGroupFactory
	{
		private readonly int _nonOwningSetCapacity;

		public NormalGroupFactory(int nonOwningSetCapacity = Constants.DefaultSetCapacity)
		{
			_nonOwningSetCapacity = nonOwningSetCapacity;
		}

		public IOwningGroup CreateOwningGroup(IReadOnlyList<SparseSet> owned, IReadOnlyList<SparseSet> include = null, IReadOnlyList<SparseSet> exclude = null)
		{
			return new OwningGroup(owned, include, exclude);
		}

		public IGroup CreateNonOwningGroup(IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude = null)
		{
			return new NonOwningGroup(include, exclude, _nonOwningSetCapacity);
		}
	}
}
