namespace Massive
{
	public class NonOwningGroup<TFirst, TSecond>
		where TFirst : struct
		where TSecond : struct
	{
		private ISet First { get; }
		private ISet Second { get; }
		private ISet Group { get; }

		public int GroupSize => Group.AliveCount;

		public NonOwningGroup(IRegistry registry)
		{
			First = registry.Any<TFirst>();
			Second = registry.Any<TSecond>();

			Group = registry.SetFactory.CreateSparseSet();
			registry.AllSets.Add(Group);

			First.AfterAdded += OnAdded;
			Second.AfterAdded += OnAdded;

			First.BeforeDeleted += OnBeforeDeleted;
			Second.BeforeDeleted += OnBeforeDeleted;

			InitGroup();
		}

		private void InitGroup()
		{
			var minimal = ViewUtils.GetMinimalSet(new[] { First, Second });
			foreach (var id in minimal.AliveIds)
			{
				OnAdded(id);
			}
		}

		private void OnAdded(int id)
		{
			if (First.IsAlive(id) && Second.IsAlive(id))
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