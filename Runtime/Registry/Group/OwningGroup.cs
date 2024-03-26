using System;
using System.Linq;

namespace Massive
{
	public class OwningGroup : IGroup
	{
		public ISet[] Owned { get; }
		public ISet[] Other { get; }
		public ISet[] All { get; }

		public bool IsSorted { get; protected set; }

		public int Length { get; private set; }

		public OwningGroup(ISet[] owned, ISet[] other = null)
		{
			Owned = owned;
			Other = other ?? Array.Empty<ISet>();

			All = Owned.Concat(Other).ToArray();

			foreach (var set in All)
			{
				set.AfterAdded += OnAfterAdded;
				set.BeforeDeleted += OnBeforeDeleted;
			}
		}

		public void Wake()
		{
			if (IsSorted)
			{
				return;
			}

			SortOwned();
			IsSorted = true;
		}

		private void SortOwned()
		{
			Length = 0;
			var minimal = SetUtils.GetMinimalSet(All);
			foreach (var id in minimal.AliveIds)
			{
				OnAfterAdded(id);
			}
		}

		private void OnAfterAdded(int id)
		{
			if (IsSorted && SetUtils.AliveInAll(id, All))
			{
				SwapEntry(id, Length);
				Length += 1;
			}
		}

		private void OnBeforeDeleted(int id)
		{
			if (IsSorted && Owned[0].TryGetDense(id, out var dense) && dense < Length)
			{
				Length -= 1;
				SwapEntry(id, Length);
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