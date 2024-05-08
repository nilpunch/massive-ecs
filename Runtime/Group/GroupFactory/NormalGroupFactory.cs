using System.Collections.Generic;

namespace Massive
{
	public class NormalGroupFactory : IGroupFactory
	{
		private readonly int _nonOwningDataCapacity;

		public NormalGroupFactory(int nonOwningDataCapacity = Constants.DataCapacity)
		{
			_nonOwningDataCapacity = nonOwningDataCapacity;
		}

		public IOwningGroup CreateOwningGroup(IReadOnlyList<ISet> owned, IReadOnlyList<IReadOnlySet> include = null, IReadOnlyList<IReadOnlySet> exclude = null)
		{
			return new OwningGroup(owned, include, exclude);
		}

		public IGroup CreateNonOwningGroup(IReadOnlyList<IReadOnlySet> include, IReadOnlyList<IReadOnlySet> exclude = null)
		{
			return new NonOwningGroup(include, exclude, _nonOwningDataCapacity);
		}
	}
}
