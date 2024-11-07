using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class OwningGroup : Group
	{
		private SparseSet[] OwnedPlusIncluded { get; }

		private SparseSet[] OwnedMinusFirstPlusIncluded { get; }

		public SparseSet[] Owned { get; }

		public SparseSet[] Included { get; }

		public SparseSet[] Excluded { get; }

		public OwningGroup Extended { get; set; }

		public OwningGroup Base { get; set; }

		public OwningGroup(IReadOnlyList<SparseSet> owned, IReadOnlyList<SparseSet> included = null, IReadOnlyList<SparseSet> excluded = null)
		{
			Owned = owned.ToArray();
			Included = (included ?? Array.Empty<SparseSet>()).ToArray();
			Excluded = (excluded ?? Array.Empty<SparseSet>()).ToArray();

			MainSet = Owned[0];

			OwnedPlusIncluded = Owned.Concat(Included).ToArray();
			OwnedMinusFirstPlusIncluded = OwnedPlusIncluded.Skip(1).ToArray();

			foreach (var set in Owned)
			{
				set.PackingModeChanged += OnPackingModeChanged;
			}
			
			foreach (var set in OwnedPlusIncluded)
			{
				set.AfterAssigned += AddToGroup;
				set.BeforeUnassigned += RemoveFromGroup;
			}

			foreach (var set in Excluded)
			{
				set.AfterAssigned += RemoveFromGroup;
				set.BeforeUnassigned += AddToGroupBeforeUnassignedFromExcluded;
			}
		}

		private void OnPackingModeChanged(PackingMode packingMode)
		{
			throw new Exception("Set is owned by a group and does not support change of packing mode.");
		}

		public override void EnsureSynced()
		{
			if (IsSynced)
			{
				return;
			}

			IsSynced = true;

			Count = 0;
			var minimal = SetHelpers.GetMinimalSet(OwnedPlusIncluded);
			for (var i = 0; i < minimal.Count; i++)
			{
				AddToGroup(minimal.Ids[i]);
			}
		}

		public override void Desync()
		{
			IsSynced = false;
		}

		public override bool IsOwning(SparseSet set)
		{
			return Owned.Contains(set);
		}

		public bool ExtendsGroup(IReadOnlyList<SparseSet> owned, IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude)
		{
			return Owned.Contains(owned) && Included.Contains(include) && Excluded.Contains(exclude);
		}

		public bool BaseForGroup(IReadOnlyList<SparseSet> owned, IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude)
		{
			return owned.Contains(Owned) && include.Contains(Included) && exclude.Contains(Excluded);
		}

		public void AddGroupAfterThis(OwningGroup group)
		{
			if (Extended != null)
			{
				Extended.Base = group;
				group.Extended = Extended;
			}

			group.Base = this;
			Extended = group;
		}

		public void AddGroupBeforeThis(OwningGroup group)
		{
			if (Base != null)
			{
				Base.Extended = group;
				group.Base = Base;
			}

			group.Extended = this;
			Base = group;
		}

		public void AddToGroup(int id)
		{
			Base?.AddToGroup(id);

			if (IsSynced && Owned[0].TryGetIndex(id, out var index) && index >= Count && SetHelpers.AssignedInAll(id, OwnedMinusFirstPlusIncluded)
			    && SetHelpers.NotAssignedInAll(id, Excluded))
			{
				SwapEntry(id, Count);
				Count += 1;
			}
		}

		public void RemoveFromGroup(int id)
		{
			Extended?.RemoveFromGroup(id);

			if (IsSynced && Owned[0].TryGetIndex(id, out var index) && index < Count)
			{
				Count -= 1;
				SwapEntry(id, Count);
			}
		}

		public void AddToGroupBeforeUnassignedFromExcluded(int id)
		{
			Base?.AddToGroupBeforeUnassignedFromExcluded(id);

			// Applies only when removed from the last remaining exclude set
			if (IsSynced && Owned[0].TryGetIndex(id, out var index) && index >= Count && SetHelpers.AssignedInAll(id, OwnedMinusFirstPlusIncluded)
			    && SetHelpers.CountAssignedInAll(id, Excluded) == 1)
			{
				SwapEntry(id, Count);
				Count += 1;
			}
		}

		private void SwapEntry(int id, int swapDense)
		{
			for (var i = 0; i < Owned.Length; i++)
			{
				var set = Owned[i];
				set.SwapPacked(set.GetIndex(id), swapDense);
			}
		}
	}
}
