using System.Collections.Generic;

namespace Massive
{
	public interface IOwningGroup : IGroup
	{
		IOwningGroup Extended { get; set; }

		IOwningGroup Base { get; set; }

		bool ExtendsGroup(IReadOnlyList<SparseSet> owned, IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude);

		bool BaseForGroup(IReadOnlyList<SparseSet> owned, IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude);

		void AddToGroup(int id);

		void RemoveFromGroup(int id);

		void AddToGroupBeforeUnassignedFromExcluded(int id);

		void AddGroupAfterThis(IOwningGroup group)
		{
			if (Extended != null)
			{
				Extended.Base = group;
				group.Extended = Extended;
			}

			group.Base = this;
			Extended = group;
		}

		void AddGroupBeforeThis(IOwningGroup group)
		{
			if (Base != null)
			{
				Base.Extended = group;
				group.Base = Base;
			}

			group.Extended = this;
			Base = group;
		}
	}
}
