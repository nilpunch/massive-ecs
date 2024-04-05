namespace Massive
{
	public interface IOwningGroup : IGroup
	{
		IOwningGroup Extended { get; set; }

		IOwningGroup Base { get; set; }

		bool ExtendsGroup(ISet[] owned, IReadOnlySet[] include, IReadOnlySet[] exclude);

		bool BaseForGroup(ISet[] owned, IReadOnlySet[] include, IReadOnlySet[] exclude);

		void AddToGroup(int id);

		void RemoveFromGroup(int id);

		void AddToGroupBeforeRemovedFromExcluded(int id);

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