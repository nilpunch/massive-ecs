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

			First.Added += OnAdded;
			Second.Added += OnAdded;

			First.Removed += OnRemoved;
			Second.Removed += OnRemoved;

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

		private void OnRemoved((int Id, int Dense) entry)
		{
			if (entry.Dense < GroupSize)
			{
				GroupSize -= 1;
				SwapEntry(entry.Id, GroupSize);
			}
		}

		protected abstract void SwapEntry(int entryId, int position);
	}
}