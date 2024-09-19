using System;
using System.Collections.Generic;
using System.Linq;

namespace Massive
{
	public class NonOwningGroup : IGroup
	{
		private ArraySegment<SparseSet> Include { get; }

		private ArraySegment<SparseSet> Exclude { get; }

		public SparseSet GroupSet { get; }

		public bool IsSynced { get; protected set; }

		public ReadOnlySpan<int> Ids => GroupSet.Ids.AsSpan(0, GroupSet.Count);

		public NonOwningGroup(IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude = null, int setCapacity = Constants.DefaultCapacity)
			: this(new SparseSet(setCapacity), include, exclude)
		{
		}

		protected NonOwningGroup(SparseSet groupSet, IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude = null)
		{
			GroupSet = groupSet;
			Include = (include ?? Array.Empty<SparseSet>()).ToArray();
			Exclude = (exclude ?? Array.Empty<SparseSet>()).ToArray();

			foreach (var set in Include)
			{
				set.AfterAssigned += AddToGroup;
				set.BeforeUnassigned += RemoveFromGroup;
			}

			foreach (var set in Exclude)
			{
				set.AfterAssigned += RemoveFromGroup;
				set.BeforeUnassigned += AddToGroupBeforeUnassignedFromExcluded;
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
			var minimal = SetHelpers.GetMinimalSet(Include);
			for (var i = 0; i < minimal.Count; i++)
			{
				AddToGroup(minimal.Ids[i]);
			}
		}

		public bool IsOwning(SparseSet set)
		{
			return false;
		}

		private void AddToGroup(int id)
		{
			if (IsSynced && SetHelpers.AssignedInAll(id, Include) && SetHelpers.NotAssignedInAll(id, Exclude))
			{
				GroupSet.Assign(id);
			}
		}

		private void RemoveFromGroup(int id)
		{
			if (IsSynced)
			{
				GroupSet.Unassign(id);
			}
		}

		private void AddToGroupBeforeUnassignedFromExcluded(int id)
		{
			// Applies only when removed from the last remaining exclude set
			if (IsSynced && SetHelpers.AssignedInAll(id, Include) && SetHelpers.CountAssignedInAll(id, Exclude) == 1)
			{
				GroupSet.Assign(id);
			}
		}
	}
}
