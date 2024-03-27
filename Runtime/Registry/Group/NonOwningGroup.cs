using System;
using System.Collections.Generic;

namespace Massive
{
	public class NonOwningGroup : IGroup
	{
		private IFilter Filter { get; }
		private IReadOnlyList<IReadOnlySet> Other { get; }
		protected ISet GroupSet { get; }

		protected bool IsSynced { get; set; }

		public ReadOnlySpan<int> GroupIds => GroupSet.AliveIds;

		public NonOwningGroup(IReadOnlyList<IReadOnlySet> other, IFilter filter = null, int dataCapacity = Constants.DataCapacity)
			: this(other, new SparseSet(dataCapacity), filter)
		{
		}

		protected NonOwningGroup(IReadOnlyList<IReadOnlySet> other, ISet groupSet, IFilter filter = null)
		{
			Other = other;
			GroupSet = groupSet;
			Filter = filter ?? EmptyFilter.Instance;
		}

		public bool IsOwning(IReadOnlySet set)
		{
			return false;
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
			if (IsSynced && SetUtils.AliveInAll(entityId, Other) && Filter.Contains(entityId))
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