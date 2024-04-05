using System;
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

		protected bool IsSynced { set; get; }

		public IOwningGroup Extended { get; set; }

		public IOwningGroup Base { get; set; }

		public ReadOnlySpan<int> Ids => Owned[0].AliveIds.Slice(0, GroupLength);

		public OwningGroup(ISet[] owned, IReadOnlySet[] include = null, IReadOnlySet[] exclude = null)
		{
			Owned = owned;
			Include = include ?? Array.Empty<IReadOnlySet>();
			Exclude = exclude ?? Array.Empty<IReadOnlySet>();

			OwnedPlusIncluded = Owned.Concat(Include).ToArray();
			OwnedMinusFirstPlusIncluded = OwnedPlusIncluded.Skip(1).ToArray();

			foreach (var set in OwnedPlusIncluded)
			{
				set.AfterAdded += AddToGroup;
				set.BeforeRemoved += RemoveFromGroup;
			}

			foreach (var set in Exclude)
			{
				set.AfterAdded += RemoveFromGroup;
				set.BeforeRemoved += AddToGroupBeforeRemovedFromExcluded;
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
			var minimal = SetHelpers.GetMinimalSet(OwnedPlusIncluded).AliveIds;
			foreach (var id in minimal)
			{
				AddToGroup(id);
			}
		}

		public bool IsOwning(IReadOnlySet set)
		{
			return Owned.Contains(set);
		}

		public bool ExtendsGroup(ISet[] owned, IReadOnlySet[] include, IReadOnlySet[] exclude)
		{
			return Owned.Contains(owned) && Include.Contains(include) && Exclude.Contains(exclude);
		}

		public bool BaseForGroup(ISet[] owned, IReadOnlySet[] include, IReadOnlySet[] exclude)
		{
			return owned.Contains(Owned) && include.Contains(Include) && exclude.Contains(Exclude);
		}

		public void AddToGroup(int id)
		{
			Base?.AddToGroup(id);

			if (IsSynced && Owned[0].TryGetDense(id, out var dense) && dense >= GroupLength && SetHelpers.AliveInAll(id, OwnedMinusFirstPlusIncluded)
			    && SetHelpers.NotAliveInAll(id, Exclude))
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

		public void AddToGroupBeforeRemovedFromExcluded(int id)
		{
			Base?.AddToGroupBeforeRemovedFromExcluded(id);

			// Applies only when removed from the last remaining exclude set
			if (IsSynced && Owned[0].TryGetDense(id, out var dense) && dense >= GroupLength && SetHelpers.AliveInAll(id, OwnedMinusFirstPlusIncluded)
			    && SetHelpers.CountAliveInAll(id, Exclude) == 1)
			{
				SwapEntry(id, GroupLength);
				GroupLength += 1;
			}
		}

		private void SwapEntry(int entryId, int swapDense)
		{
			foreach (var set in Owned)
			{
				set.SwapDense(set.GetDense(entryId), swapDense);
			}
		}
	}
}