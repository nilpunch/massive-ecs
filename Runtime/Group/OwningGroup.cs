using System;
using System.Collections.Generic;
using System.Linq;

namespace Massive
{
	public class OwningGroup : IOwningGroup
	{
		private IReadOnlySet[] OwnedPlusIncluded { get; }

		private IReadOnlySet[] OwnedMinusFirstPlusIncluded { get; }

		private ISet[] Owned { get; }

		private IReadOnlySet[] Include { get; }

		private IReadOnlySet[] Exclude { get; }

		protected int GroupLength { get; set; }

		public bool IsSynced { get; protected set; }

		public IOwningGroup Extended { get; set; }

		public IOwningGroup Base { get; set; }

		public ReadOnlySpan<int> Ids => Owned[0].Ids.Slice(0, GroupLength);

		public OwningGroup(IReadOnlyList<ISet> owned, IReadOnlyList<IReadOnlySet> include = null, IReadOnlyList<IReadOnlySet> exclude = null)
		{
			Owned = owned.ToArray();
			Include = (include ?? Array.Empty<IReadOnlySet>()).ToArray();
			Exclude = (exclude ?? Array.Empty<IReadOnlySet>()).ToArray();

			OwnedPlusIncluded = Owned.Concat(Include).ToArray();
			OwnedMinusFirstPlusIncluded = OwnedPlusIncluded.Skip(1).ToArray();

			foreach (var set in OwnedPlusIncluded)
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

			GroupLength = 0;
			var minimal = SetHelpers.GetMinimalSet(OwnedPlusIncluded).Ids;
			for (var i = 0; i < minimal.Length; i++)
			{
				AddToGroup(minimal[i]);
			}
		}

		public bool IsOwning(IReadOnlySet set)
		{
			return Owned.Contains(set);
		}

		public bool ExtendsGroup(IReadOnlyList<ISet> owned, IReadOnlyList<IReadOnlySet> include, IReadOnlyList<IReadOnlySet> exclude)
		{
			return Owned.Contains(owned) && Include.Contains(include) && Exclude.Contains(exclude);
		}

		public bool BaseForGroup(IReadOnlyList<ISet> owned, IReadOnlyList<IReadOnlySet> include, IReadOnlyList<IReadOnlySet> exclude)
		{
			return owned.Contains(Owned) && include.Contains(Include) && exclude.Contains(Exclude);
		}

		public void AddToGroup(int id)
		{
			Base?.AddToGroup(id);

			if (IsSynced && Owned[0].TryGetDense(id, out var dense) && dense >= GroupLength && SetHelpers.AssignedInAll(id, OwnedMinusFirstPlusIncluded)
			    && SetHelpers.NotAssignedInAll(id, Exclude))
			{
				SwapEntry(id, GroupLength);
				GroupLength += 1;
			}
		}

		public void RemoveFromGroup(int id)
		{
			Extended?.RemoveFromGroup(id);

			if (IsSynced && Owned[0].TryGetDense(id, out var dense) && dense < GroupLength)
			{
				GroupLength -= 1;
				SwapEntry(id, GroupLength);
			}
		}

		public void AddToGroupBeforeUnassignedFromExcluded(int id)
		{
			Base?.AddToGroupBeforeUnassignedFromExcluded(id);

			// Applies only when removed from the last remaining exclude set
			if (IsSynced && Owned[0].TryGetDense(id, out var dense) && dense >= GroupLength && SetHelpers.AssignedInAll(id, OwnedMinusFirstPlusIncluded)
			    && SetHelpers.CountAssignedInAll(id, Exclude) == 1)
			{
				SwapEntry(id, GroupLength);
				GroupLength += 1;
			}
		}

		private void SwapEntry(int id, int swapDense)
		{
			for (var i = 0; i < Owned.Length; i++)
			{
				var set = Owned[i];
				set.SwapDense(set.GetDense(id), swapDense);
			}
		}
	}
}
