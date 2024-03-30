using System;

namespace Massive
{
	public class NonOwningGroup : IGroup
	{
		protected ISet GroupSet { get; }

		protected bool IsSynced { get; set; }


		public ISet[] Owned => Array.Empty<ISet>();

		public IReadOnlySet[] Include { get; }

		public IReadOnlySet[] Exclude { get; }

		public ReadOnlySpan<int> GroupIds => GroupSet.AliveIds;

		public NonOwningGroup(IReadOnlySet[] include, IReadOnlySet[] exclude = null, int dataCapacity = Constants.DataCapacity)
			: this(new SparseSet(dataCapacity), include, exclude) { }

		protected NonOwningGroup(ISet groupSet, IReadOnlySet[] include, IReadOnlySet[] exclude = null)
		{
			GroupSet = groupSet;
			Include = include ?? Array.Empty<IReadOnlySet>();
			Exclude = exclude ?? Array.Empty<IReadOnlySet>();

			foreach (var set in Include)
			{
				set.AfterAdded += AddToGroup;
				set.BeforeDeleted += RemoveFromGroup;
			}

			foreach (var set in Exclude)
			{
				set.AfterAdded += RemoveFromGroup;
				set.BeforeDeleted += AddToGroupWhenRemovedFromFilter;
			}
		}

		public void EnsureSynced()
		{
			if (IsSynced)
			{
				return;
			}

			IsSynced = true;

			GroupSet.Clear();
			var minimal = SetHelpers.GetMinimalSet(Include).AliveIds;
			foreach (var id in minimal)
			{
				AddToGroup(id);
			}
		}

		public bool IsOwning(IReadOnlySet set)
		{
			return false;
		}

		public bool ExtendsGroup(IGroup group)
		{
			return Include.Contains(group.Include) && Exclude.Contains(group.Exclude);
		}

		public bool ExtendsGroup(ISet[] owned, IReadOnlySet[] include, IReadOnlySet[] exclude)
		{
			return Include.Contains(include) && Exclude.Contains(exclude);
		}

		public bool BaseForGroup(ISet[] owned, IReadOnlySet[] include, IReadOnlySet[] exclude)
		{
			return include.Contains(Include) && exclude.Contains(Exclude);
		}

		private void AddToGroup(int id)
		{
			if (IsSynced && SetHelpers.AliveInAll(id, Include) && SetHelpers.NotAliveInAll(id, Exclude))
			{
				GroupSet.Ensure(id);
			}
		}

		private void RemoveFromGroup(int id)
		{
			if (IsSynced)
			{
				GroupSet.Delete(id);
			}
		}

		private void AddToGroupWhenRemovedFromFilter(int id)
		{
			if (IsSynced && SetHelpers.AliveInAll(id, Include) && SetHelpers.CountAliveInAll(id, Exclude) == 1)
			{
				GroupSet.Ensure(id);
			}
		}
	}
}