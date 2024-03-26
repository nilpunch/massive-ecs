namespace Massive
{
	public class NonOwningGroup : IGroup
	{
		public IFilter Filter { get; }
		public ISet[] Other { get; }
		public ISet Group { get; }

		public bool IsWaken { get; protected set; }

		public int Length => Group.AliveCount;

		public NonOwningGroup(ISet[] other, int dataCapacity = Constants.DataCapacity, IFilter filter = null)
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

		public void Wake()
		{
			if (IsWaken)
			{
				return;
			}

			SortGroup();
			IsWaken = true;
		}

		private void SortGroup()
		{
			Group.Clear();
			var minimal = SetUtils.GetMinimalSet(Other).AliveIds;
			for (var i = 0; i < minimal.Length; i++)
			{
				OnAfterAdded(minimal[i]);
			}
		}

		private void OnAfterAdded(int id)
		{
			if (IsWaken && SetUtils.AliveInAll(id, Other) && Filter.Contains(id))
			{
				Group.Ensure(id);
			}
		}

		private void OnBeforeDeleted(int id)
		{
			if (IsWaken)
			{
				Group.Delete(id);
			}
		}
	}
}