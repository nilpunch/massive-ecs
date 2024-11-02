using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class NonOwningGroup : Group
	{
		public SparseSet[] Included { get; }

		public SparseSet[] Excluded { get; }

		private Entities Entities { get; }

		public NonOwningGroup(IReadOnlyList<SparseSet> included, IReadOnlyList<SparseSet> excluded = null, Entities entities = null)
			: this(new SparseSet(), included, excluded, entities)
		{
		}

		protected NonOwningGroup(SparseSet groupSet, IReadOnlyList<SparseSet> included, IReadOnlyList<SparseSet> excluded = null, Entities entities = null)
		{
			MainSet = groupSet;
			Included = (included ?? Array.Empty<SparseSet>()).ToArray();
			Excluded = (excluded ?? Array.Empty<SparseSet>()).ToArray();
			Entities = entities;

			foreach (var set in Included)
			{
				set.AfterAssigned += AddToGroup;
				set.BeforeUnassigned += RemoveFromGroup;
			}

			foreach (var set in Excluded)
			{
				set.AfterAssigned += RemoveFromGroup;
				set.BeforeUnassigned += AddToGroupBeforeUnassignedFromExcluded;
			}

			if (Entities != null)
			{
				Entities.AfterCreated += AddToGroup;
				Entities.BeforeDestroyed += RemoveFromGroup;
			}
		}

		public override void EnsureSynced()
		{
			if (IsSynced)
			{
				return;
			}

			IsSynced = true;

			MainSet.Clear();
			SyncCount();
			IdsSource minimal = Included.Length == 0 ? Entities : SetHelpers.GetMinimalSet(Included);
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
			return false;
		}

		private void AddToGroup(int id)
		{
			if (IsSynced
			    && (Included.Length == 0 || SetHelpers.AssignedInAll(id, Included))
			    && (Excluded.Length == 0 || SetHelpers.NotAssignedInAll(id, Excluded)))
			{
				MainSet.Assign(id);
				SyncCount();
			}
		}

		private void RemoveFromGroup(int id)
		{
			if (IsSynced)
			{
				MainSet.Unassign(id);
				SyncCount();
			}
		}

		private void AddToGroupBeforeUnassignedFromExcluded(int id)
		{
			// Applies only when removed from the last remaining exclude set
			if (IsSynced
			    && (Included.Length == 0 || SetHelpers.AssignedInAll(id, Included))
			    && SetHelpers.CountAssignedInAll(id, Excluded) == 1)
			{
				MainSet.Assign(id);
				SyncCount();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SyncCount()
		{
			Count = MainSet.Count;
		}
	}
}
