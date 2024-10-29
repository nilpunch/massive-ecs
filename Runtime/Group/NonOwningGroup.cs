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
		private SparseSet[] Include { get; }

		private SparseSet[] Exclude { get; }

		public override bool IsSynced { get; protected set; }

		public override SparseSet MainSet { get; }

		public NonOwningGroup(IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude = null)
			: this(new SparseSet(), include, exclude)
		{
		}

		protected NonOwningGroup(SparseSet groupSet, IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude = null)
		{
			MainSet = groupSet;
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

		public override void EnsureSynced()
		{
			if (IsSynced)
			{
				return;
			}

			IsSynced = true;

			MainSet.Clear();
			var minimal = SetHelpers.GetMinimalSet(Include);
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
			if (IsSynced && SetHelpers.AssignedInAll(id, Include) && SetHelpers.NotAssignedInAll(id, Exclude))
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
			if (IsSynced && SetHelpers.AssignedInAll(id, Include) && SetHelpers.CountAssignedInAll(id, Exclude) == 1)
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
