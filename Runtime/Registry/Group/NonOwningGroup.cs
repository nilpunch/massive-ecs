using System;

namespace Massive
{
	public class NonOwningGroup : IGroup
	{
		private IFilter Filter { get; }
		private ISet[] Other { get; }
		protected ISet GroupSet { get; }

		protected bool IsSynced { get; set; }

		public ReadOnlySpan<int> GroupIds => GroupSet.AliveIds;

		public NonOwningGroup(ISet[] other, IFilter filter = null, int dataCapacity = Constants.DataCapacity)
			: this(other, new SparseSet(dataCapacity), filter)
		{
		}

		protected NonOwningGroup(ISet[] other, ISet groupSet, IFilter filter = null)
		{
			Other = other;
			GroupSet = groupSet;
			Filter = filter ?? EmptyFilter.Instance;

			foreach (var set in Other)
			{
				set.AfterAdded += OnAfterAdded;
				set.BeforeDeleted += OnBeforeDeleted;
			}
		}

		public bool IsOwning(ISet set)
		{
			return false;
		}

		public void EnsureSynced()
		{
			if (IsSynced)
			{
				return;
			}

			GroupSet.Clear();
			var minimal = SetUtils.GetMinimalSet(Other).AliveIds;
			foreach (var id in minimal)
			{
				OnAfterAdded(id);
			}

			IsSynced = true;
		}

		private void OnAfterAdded(int id)
		{
			if (IsSynced && SetUtils.AliveInAll(id, Other) && Filter.Contains(id))
			{
				GroupSet.Ensure(id);
			}
		}

		private void OnBeforeDeleted(int id)
		{
			if (IsSynced)
			{
				GroupSet.Delete(id);
			}
		}
	}
}