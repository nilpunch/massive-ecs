namespace Massive
{
	public class NonOwningGroup
	{
		public ISet[] Other { get; }
		public ISet Group { get; }

		public int GroupSize => Group.AliveCount;

		public NonOwningGroup(IRegistry registry, ISet[] other)
		{
			Other = other;

			Group = registry.SetFactory.CreateSparseSet();
			registry.AllSets.Add(Group);

			foreach (var set in Other)
			{
				set.AfterAdded += OnAfterAdded;
				set.BeforeDeleted += OnBeforeDeleted;
			}

			SortGroup();
		}

		private void SortGroup()
		{
			var minimal = SetUtils.GetMinimalSet(Other);
			foreach (var id in minimal.AliveIds)
			{
				OnAfterAdded(id);
			}
		}

		private void OnAfterAdded(int id)
		{
			if (SetUtils.AliveInAll(id, Other))
			{
				Group.Ensure(id);
			}
		}

		private void OnBeforeDeleted(int id)
		{
			Group.Delete(id);
		}
	}
}