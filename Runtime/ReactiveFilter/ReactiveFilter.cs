using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class ReactiveFilter
	{
		public SparseSet[] Included { get; }

		public SparseSet[] Excluded { get; }

		private Entities Entities { get; }

		public int Count => Set.Count;

		public SparseSet Set { get; }

		public bool IsSynced { get; protected set; }

		public ReactiveFilter(IReadOnlyList<SparseSet> included, IReadOnlyList<SparseSet> excluded = null, Entities entities = null)
			: this(new SparseSet(), included, excluded, entities)
		{
		}

		protected ReactiveFilter(SparseSet groupSet, IReadOnlyList<SparseSet> included, IReadOnlyList<SparseSet> excluded = null, Entities entities = null)
		{
			Set = groupSet;
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

		public void EnsureSynced()
		{
			if (IsSynced)
			{
				return;
			}

			IsSynced = true;

			Set.Clear();
			IdsSource minimal = Included.Length == 0 ? Entities : SetHelpers.GetMinimalSet(Included);
			for (var i = 0; i < minimal.Count; i++)
			{
				var id = minimal.Ids[i];
				if (id >= 0)
				{
					AddToGroup(id);
				}
			}
		}

		public void Desync()
		{
			IsSynced = false;
		}

		public bool IsIncluding(SparseSet set)
		{
			return Included.Contains(set);
		}

		private void AddToGroup(int id)
		{
			if (IsSynced
			    && (Included.Length == 0 || SetHelpers.AssignedInAll(id, Included))
			    && (Excluded.Length == 0 || SetHelpers.NotAssignedInAll(id, Excluded)))
			{
				Set.Assign(id);
			}
		}

		private void RemoveFromGroup(int id)
		{
			if (IsSynced)
			{
				Set.Unassign(id);
			}
		}

		private void AddToGroupBeforeUnassignedFromExcluded(int id)
		{
			// Applies only when removed from the last remaining exclude set
			if (IsSynced
			    && (Included.Length == 0 || SetHelpers.AssignedInAll(id, Included))
			    && SetHelpers.CountAssignedInAll(id, Excluded) == 1)
			{
				Set.Assign(id);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IdsSourceEnumerator GetEnumerator()
		{
			return new IdsSourceEnumerator(Set);
		}
	}
}
