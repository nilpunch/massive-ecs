using System;
using System.Collections.Generic;

namespace Massive
{
	public class NonOwningGroup : IGroup
	{
		protected ISet GroupSet { get; }
		protected bool IsSynced { get; set; }

		public IFilter Filter { get; }

		public ISet[] Owned => Array.Empty<ISet>();

		public IReadOnlySet[] Other { get; }

		public IGroup ExtendedGroup { get; set; }
		public IGroup ParentGroup { get; set; }

		public ReadOnlySpan<int> GroupIds => GroupSet.AliveIds;

		public NonOwningGroup(IReadOnlySet[] other, IFilter filter = null, int dataCapacity = Constants.DataCapacity)
			: this(other, new SparseSet(dataCapacity), filter)
		{
		}

		protected NonOwningGroup(IReadOnlySet[] other, ISet groupSet, IFilter filter = null)
		{
			Other = other;
			GroupSet = groupSet;
			Filter = filter ?? EmptyFilter.Instance;
		}

		public bool IsOwning(IReadOnlySet set)
		{
			return false;
		}

		public bool ExtendsGroup(IGroup group)
		{
			return Other.Contains(group.Other) && Filter.Contains(group.Filter);
		}

		public bool ExtendsGroup(ISet[] owned, IReadOnlySet[] other, IFilter filter)
		{
			return Other.Contains(other) && Filter.Contains(filter);
		}

		public bool BaseForGroup(ISet[] owned, IReadOnlySet[] other, IFilter filter)
		{
			return other.Contains(Other) && filter.Contains(Filter);
		}

		public void EnsureSynced()
		{
			if (IsSynced)
			{
				return;
			}

			IsSynced = true;

			GroupSet.Clear();
			var minimal = SetUtils.GetMinimalSet(Other).AliveIds;
			foreach (var id in minimal)
			{
				AddEntity(id);
			}
		}

		public void AddEntity(int entityId)
		{
			if (IsSynced && SetUtils.AliveInAll(entityId, Other) && Filter.ContainsId(entityId))
			{
				GroupSet.Ensure(entityId);
			}
		}

		public void RemoveEntity(int entityId)
		{
			if (IsSynced)
			{
				GroupSet.Delete(entityId);
			}
		}
	}
}