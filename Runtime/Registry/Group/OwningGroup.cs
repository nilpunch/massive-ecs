namespace Massive
{
	public abstract class OwningGroup<TFirst, TSecond>
		where TFirst : struct
		where TSecond : struct
	{
		public ISet First { get; }
		public ISet Second { get; }

		public int GroupSize { get; private set; }

		public OwningGroup(IRegistry registry)
		{
			First = registry.Any<TFirst>();
			Second = registry.Any<TSecond>();

			First.AfterAdded += OnAdded;
			Second.AfterAdded += OnAdded;

			First.BeforeDeleted += OnBeforeDeleted;
			Second.BeforeDeleted += OnBeforeDeleted;

			SortDense();
		}

		private void SortDense()
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
				SwapEntry(id, GroupSize);
				GroupSize += 1;
			}
		}

		private void OnBeforeDeleted(int id)
		{
			if (First.TryGetDense(id, out var dense) && dense < GroupSize)
			{
				GroupSize -= 1;
				SwapEntry(id, GroupSize);
			}
		}

		protected abstract void SwapEntry(int entryId, int position);
	}
}