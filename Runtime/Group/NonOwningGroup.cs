using System;
using System.Collections.Generic;
using System.Linq;

namespace Massive
{
	public class NonOwningGroup : IGroup
	{
		private IReadOnlyList<IReadOnlySet> Include { get; }

		private IReadOnlyList<IReadOnlySet> Exclude { get; }

		public ISet GroupSet { get; }

		public bool IsSynced { get; protected set; }

		public ReadOnlyPackedSpan<int> Ids => GroupSet.Ids;

		public NonOwningGroup(IReadOnlyList<IReadOnlySet> include, IReadOnlyList<IReadOnlySet> exclude = null, int dataCapacity = Constants.DataCapacity)
			: this(new SparseSet(dataCapacity), include, exclude)
		{
		}

		protected NonOwningGroup(ISet groupSet, IReadOnlyList<IReadOnlySet> include, IReadOnlyList<IReadOnlySet> exclude = null)
		{
			GroupSet = groupSet;
			Include = (include ?? Array.Empty<IReadOnlySet>()).ToArray();
			Exclude = (exclude ?? Array.Empty<IReadOnlySet>()).ToArray();

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
			var minimal = SetHelpers.GetMinimalSet(Include).Ids;
			for (var i = 0; i < minimal.Length; i++)
			{
				AddToGroup(minimal[i]);
			}
		}

		public bool IsOwning(IReadOnlySet set)
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
