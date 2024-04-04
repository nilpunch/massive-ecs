using System;

namespace Massive
{
	public interface IGroup
	{
		IGroup Extended { get; set; }

		IGroup Base { get; set; }

		ReadOnlySpan<int> Ids { get; }

		int Length => Ids.Length;

		void EnsureSynced();

		bool IsOwning(IReadOnlySet set);

		bool ExtendsGroup(ISet[] owned, IReadOnlySet[] include, IReadOnlySet[] exclude);

		bool BaseForGroup(ISet[] owned, IReadOnlySet[] include, IReadOnlySet[] exclude);

		void AddToGroup(int id);

		void RemoveFromGroup(int id);

		void AddToGroupBeforeRemovedFromExcluded(int id);

		void AddAfterThis(IGroup group)
		{
			if (Extended != null)
			{
				Extended.Base = group;
				group.Extended = Extended;
			}

			group.Base = this;
			Extended = group;
		}

		void AddBeforeThis(IGroup group)
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
