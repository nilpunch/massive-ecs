using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class Group
	{
		public SparseSet[] Included { get; }

		public SparseSet[] Excluded { get; }

		private Entities Entities { get; }

		public int Count => Set.Count;

		public SparseSet Set { get; }

		public bool IsSynced { get; protected set; }

		public Group(SparseSet[] included = null, SparseSet[] excluded = null, Entities entities = null)
			: this(new SparseSet(), included, excluded, entities)
		{
		}

		protected Group(SparseSet backingSet, SparseSet[] included = null, SparseSet[] excluded = null, Entities entities = null)
		{
			if ((included == null || included.Length == 0) && entities == null)
			{
				throw new Exception("You must provide at least one included or entities.");
			}

			Set = backingSet;
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

			if (Included.Length == 0)
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
			PackedSet minimal = Included.Length == 0 ? Entities : SetUtils.GetMinimalSet(Included);
			for (var i = 0; i < minimal.Count; i++)
			{
				var id = minimal.Packed[i];
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

		private void AddToGroup(int id)
		{
			if (IsSynced && SetUtils.AssignedInAll(id, Included)
				&& SetUtils.NotAssignedInAll(id, Excluded))
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
			if (IsSynced && SetUtils.AssignedInAll(id, Included)
				&& SetUtils.CountAssignedInAll(id, Excluded) == 1)
			{
				Set.Assign(id);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PackedEnumerator GetEnumerator()
		{
			return new PackedEnumerator(Set);
		}
	}
}
