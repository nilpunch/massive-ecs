using System;
using System.Linq;

namespace Massive
{
	public class OwningGroup : IGroup
	{
		public IFilter Filter { get; }
		public ISet[] Owned { get; }
		public ISet[] Other { get; }
		public ISet[] All { get; }

		public bool IsWaken { get; protected set; }

		public int Length { get; private set; }

		public OwningGroup(ISet[] owned, ISet[] other = null, IFilter filter = null)
		{
			Owned = owned;
			Other = other ?? Array.Empty<ISet>();
			Filter = filter ?? EmptyFilter.Instance;

			All = Owned.Concat(Other).ToArray();

			foreach (var set in All)
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

			SortOwned();
			IsWaken = true;
		}

		private void SortOwned()
		{
			Length = 0;
			var minimal = SetUtils.GetMinimalSet(All).AliveIds;
			for (var i = 0; i < minimal.Length; i++)
			{
				var id = minimal[i];
				OnAfterAdded(id);
			}
		}

		private void OnAfterAdded(int id)
		{
			if (IsWaken && SetUtils.AliveInAll(id, All) && Filter.Contains(id))
			{
				SwapEntry(id, Length);
				Length += 1;
			}
		}

		private void OnBeforeDeleted(int id)
		{
			if (IsWaken && Owned[0].TryGetDense(id, out var dense) && dense < Length)
			{
				Length -= 1;
				SwapEntry(id, Length);
			}
		}

		private void SwapEntry(int entryId, int swapDense)
		{
			for (var i = 0; i < Owned.Length; i++)
			{
				var set = Owned[i];
				set.SwapDense(set.GetDense(entryId), swapDense);
			}
		}
	}
}