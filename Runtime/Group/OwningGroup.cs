﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Massive
{
	public class OwningGroup : IOwningGroup
	{
		private ArraySegment<SparseSet> OwnedPlusIncluded { get; }

		private ArraySegment<SparseSet> OwnedMinusFirstPlusIncluded { get; }

		private ArraySegment<SparseSet> Owned { get; }

		private ArraySegment<SparseSet> Include { get; }

		private ArraySegment<SparseSet> Exclude { get; }

		protected int GroupLength { get; set; }

		public bool IsSynced { get; protected set; }

		public IOwningGroup Extended { get; set; }

		public IOwningGroup Base { get; set; }

		public ReadOnlySpan<int> Ids => Owned[0].Ids.AsSpan(0, GroupLength);

		public OwningGroup(IReadOnlyList<SparseSet> owned, IReadOnlyList<SparseSet> include = null, IReadOnlyList<SparseSet> exclude = null)
		{
			Owned = owned.ToArray();
			Include = (include ?? Array.Empty<SparseSet>()).ToArray();
			Exclude = (exclude ?? Array.Empty<SparseSet>()).ToArray();

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
			var minimal = SetHelpers.GetMinimalSet(OwnedPlusIncluded);
			for (var i = 0; i < minimal.Count; i++)
			{
				AddToGroup(minimal.Ids[i]);
			}
		}

		public bool IsOwning(SparseSet set)
		{
			return Owned.Contains(set);
		}

		public bool ExtendsGroup(IReadOnlyList<SparseSet> owned, IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude)
		{
			return Owned.Contains(owned) && Include.Contains(include) && Exclude.Contains(exclude);
		}

		public bool BaseForGroup(IReadOnlyList<SparseSet> owned, IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude)
		{
			return owned.Contains(Owned) && include.Contains(Include) && exclude.Contains(Exclude);
		}

		public void AddToGroup(int id)
		{
			Base?.AddToGroup(id);

			if (IsSynced && Owned[0].TryGetIndex(id, out var packed) && packed >= GroupLength && SetHelpers.AssignedInAll(id, OwnedMinusFirstPlusIncluded)
			    && SetHelpers.NotAssignedInAll(id, Exclude))
			{
				SwapEntry(id, GroupLength);
				GroupLength += 1;
			}
		}

		public void RemoveFromGroup(int id)
		{
			Extended?.RemoveFromGroup(id);

			if (IsSynced && Owned[0].TryGetIndex(id, out var packed) && packed < GroupLength)
			{
				GroupLength -= 1;
				SwapEntry(id, GroupLength);
			}
		}

		public void AddToGroupBeforeUnassignedFromExcluded(int id)
		{
			Base?.AddToGroupBeforeUnassignedFromExcluded(id);

			// Applies only when removed from the last remaining exclude set
			if (IsSynced && Owned[0].TryGetIndex(id, out var packed) && packed >= GroupLength && SetHelpers.AssignedInAll(id, OwnedMinusFirstPlusIncluded)
			    && SetHelpers.CountAssignedInAll(id, Exclude) == 1)
			{
				SwapEntry(id, GroupLength);
				GroupLength += 1;
			}
		}

		private void SwapEntry(int id, int swapDense)
		{
			for (var i = 0; i < Owned.Count; i++)
			{
				var set = Owned[i];
				set.SwapPacked(set.GetIndex(id), swapDense);
			}
		}
	}
}
