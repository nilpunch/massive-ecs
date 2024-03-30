using System;
using System.Linq;

namespace Massive
{
	public class OwningGroup : IGroup
	{
		private IReadOnlySet[] All { get; }

		protected bool IsSynced { set; get; }
		protected int GroupLength { get; set; }

		public ISet[] Owned { get; }

		public IReadOnlySet[] Include { get; }
		
		public IReadOnlySet[] Exclude { get; }

		public IGroup ExtendedGroup { get; set; }

		public ReadOnlySpan<int> GroupIds => Owned[0].AliveIds.Slice(0, GroupLength);

		public OwningGroup(ISet[] owned, IReadOnlySet[] include = null, IReadOnlySet[] exclude = null)
		{
			Owned = owned;
			Include = include ?? Array.Empty<IReadOnlySet>();
			Exclude = exclude ?? Array.Empty<IReadOnlySet>();

			All = Owned.Concat(Include).ToArray();

			foreach (var set in All)
			{
				set.AfterAdded += AddToGroup;
				set.BeforeDeleted += RemoveFromGroup;
			}

			foreach (var set in Exclude)
			{
				set.AfterAdded += RemoveFromGroup;
				set.BeforeDeleted += AddToGroupWhenRemovedFromFilter;
			}
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
				AddToGroup(id);
			}
		}

		public bool IsOwning(IReadOnlySet set)
		{
			return Owned.Contains(set);
		}

		public bool ExtendsGroup(IGroup group)
		{
			return ExtendsGroup(group.Owned, group.Include, group.Exclude);
		}

		public bool ExtendsGroup(ISet[] owned, IReadOnlySet[] include, IReadOnlySet[] exclude)
		{
			return Owned.Contains(owned) && Include.Contains(include) && Exclude.Contains(exclude);
		}

		public bool BaseForGroup(ISet[] owned, IReadOnlySet[] include, IReadOnlySet[] exclude)
		{
			return owned.Contains(Owned) && include.Contains(Include) && exclude.Contains(Exclude);
		}

		private void AddToGroup(int id)
		{
			if (IsSynced && Owned[0].GetDense(id) >= GroupLength && SetUtils.AliveInAll(id, All)
			    && SetUtils.NotAliveInAll(id, Exclude))
			{
				SwapEntry(id, GroupLength);
				GroupLength += 1;
			}
		}

		private void RemoveFromGroup(int id)
		{
			if (IsSynced && Owned[0].TryGetDense(id, out var dense) && dense < GroupLength)
			{
				GroupLength -= 1;
				SwapEntry(id, GroupLength);
			}
		}

		private void AddToGroupWhenRemovedFromFilter(int id)
		{
			if (IsSynced && Owned[0].GetDense(id) >= GroupLength && SetUtils.AliveInAll(id, All)
			    && SetUtils.CountAliveInAll(id, Exclude) == 1)
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