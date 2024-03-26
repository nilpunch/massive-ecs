namespace Massive
{
	public class NonOwningGroup : IGroup
	{
		private IFilter Filter { get; }
		private ISet[] Other { get; }
		protected ISet Group { get; }

		protected bool IsSynced { get; set; }

		public int Length => Group.AliveCount;

		public NonOwningGroup(ISet[] other, IFilter filter = null, int dataCapacity = Constants.DataCapacity)
			: this(other, new SparseSet(dataCapacity), filter)
		{
		}

		protected NonOwningGroup(ISet[] other, ISet group, IFilter filter = null)
		{
			Other = other;
			Group = group;
			Filter = filter ?? EmptyFilter.Instance;

			foreach (var set in Other)
			{
				set.AfterAdded += OnAfterAdded;
				set.BeforeDeleted += OnBeforeDeleted;
			}
		}

		public void EnsureSynced()
		{
			if (IsSynced)
			{
				return;
			}

			SortGroup();
			IsSynced = true;
		}

		private void SortGroup()
		{
			Group.Clear();
			var minimal = SetUtils.GetMinimalSet(Other).AliveIds;
			foreach (var id in minimal)
			{
				OnAfterAdded(id);
			}
		}

		private void OnAfterAdded(int id)
		{
			if (IsSynced && SetUtils.AliveInAll(id, Other) && Filter.Contains(id))
			{
				Group.Ensure(id);
			}
		}

		private void OnBeforeDeleted(int id)
		{
			if (IsSynced)
			{
				Group.Delete(id);
			}
		}
	}
}