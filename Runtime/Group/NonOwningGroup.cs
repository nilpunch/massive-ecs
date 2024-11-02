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
		public SparseSet[] Include { get; }

		public SparseSet[] Exclude { get; }

		private Entities Entities { get; }

		public NonOwningGroup(IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude = null, Entities entities = null)
			: this(new SparseSet(), include, exclude, entities)
		{
		}

		protected NonOwningGroup(SparseSet groupSet, IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude = null, Entities entities = null)
		{
			MainSet = groupSet;
			Include = (include ?? Array.Empty<SparseSet>()).ToArray();
			Exclude = (exclude ?? Array.Empty<SparseSet>()).ToArray();
			Entities = entities;

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
			IdsSource minimal = Include.Length == 0 ? Entities : SetHelpers.GetMinimalSet(Include);
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
			    && (Include.Length == 0 || SetHelpers.AssignedInAll(id, Include))
			    && (Exclude.Length == 0 || SetHelpers.NotAssignedInAll(id, Exclude)))
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
			    && (Include.Length == 0 || SetHelpers.AssignedInAll(id, Include))
			    && SetHelpers.CountAssignedInAll(id, Exclude) == 1)
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
