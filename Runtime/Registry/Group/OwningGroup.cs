using System;
using System.Linq;

namespace Massive
{
	public class OwningGroup
	{
		public ISet[] Owned { get; }
		public ISet[] Other { get; }
		public ISet[] All { get; }

		public int GroupSize { get; private set; }

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

			SortOwned();
		}

		private void SortOwned()
		{
			var minimal = SetUtils.GetMinimalSet(All);
			foreach (var id in minimal.AliveIds)
			{
				OnAfterAdded(id);
			}
		}

		private void OnAfterAdded(int id)
		{
			if (SetUtils.AliveInAll(id, All))
			{
				SwapEntry(id, GroupSize);
				GroupSize += 1;
			}
		}

		private void OnBeforeDeleted(int id)
		{
			if (Owned[0].TryGetDense(id, out var dense) && dense < GroupSize)
			{
				GroupSize -= 1;
				SwapEntry(id, GroupSize);
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