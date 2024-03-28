using System;
using System.Collections.Generic;
using System.Linq;

namespace Massive
{
	public class OwningGroup : IGroup
	{
		private IReadOnlySet[] All { get; }

		protected bool IsSynced { set; get; }
		protected int GroupLength { get; set; }

		public IFilter Filter { get; }

		public ISet[] Owned { get; }

		public IReadOnlySet[] Other { get; }

		public IGroup ExtendedGroup { get; set; }

		public ReadOnlySpan<int> GroupIds => Owned[0].AliveIds.Slice(0, GroupLength);

		public OwningGroup(ISet[] owned, IReadOnlySet[] other = null, IFilter filter = null)
		{
			Owned = owned;
			Other = other ?? Array.Empty<IReadOnlySet>();
			Filter = filter ?? EmptyFilter.Instance;

			All = Owned.Concat(Other).ToArray();

			foreach (var set in All)
			{
				set.AfterAdded += AddEntity;
				set.BeforeDeleted += RemoveEntity;
			}
		}

		public bool IsOwning(IReadOnlySet set)
		{
			return Owned.Contains(set);
		}

		public bool ExtendsGroup(IGroup group)
		{
			return ExtendsGroup(group.Owned, group.Other, group.Filter);
		}

		public bool ExtendsGroup(ISet[] owned, IReadOnlySet[] other, IFilter filter)
		{
			return Owned.Contains(owned) && Other.Contains(other) && Filter.Contains(filter);
		}

		public bool BaseForGroup(ISet[] owned, IReadOnlySet[] other, IFilter filter)
		{
			return owned.Contains(Owned) && other.Contains(Other) && filter.Contains(Filter);
		}

		public void EnsureSynced()
		{
			if (IsSynced)
			{
				return;
			}

			IsSynced = true;

			var minimal = SetUtils.GetMinimalSet(All).AliveIds;
			foreach (var id in minimal)
			{
				AddEntity(id);
			}
		}

		public void AddEntity(int id)
		{
			if (IsSynced && SetUtils.AliveInAll(id, All) && Owned[0].GetDense(id) >= GroupLength && Filter.ContainsId(id))
			{
				SwapEntry(id, GroupLength);
				GroupLength += 1;
				
				ExtendedGroup?.AddEntity(id);
			}
		}

		public void RemoveEntity(int id)
		{
			if (IsSynced && Owned[0].TryGetDense(id, out var dense) && dense < GroupLength)
			{
				ExtendedGroup?.RemoveEntity(id);
				
				GroupLength -= 1;
				SwapEntry(id, GroupLength);
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