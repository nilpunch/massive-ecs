using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class Identifiers
	{
		protected int[] Dense { get; }
		protected int[] Sparse { get; }
		protected int AliveCount { get; set; }
		protected int MaxId { get; set; }

		public Identifiers(int dataCapacity = Constants.DataCapacity)
		{
			Dense = new int[dataCapacity];
			Sparse = new int[dataCapacity];
		}

		public int CanCreateAmount => Dense.Length - AliveCount;

		public ReadOnlySpan<int> AliveIds => new ReadOnlySpan<int>(Dense, 0, AliveCount);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Create()
		{
			if (AliveCount == Dense.Length)
			{
				throw new InvalidOperationException($"Exceeded limit of data! Limit: {Dense.Length}.");
			}

			int count = AliveCount;
			AliveCount += 1;

			// If there are unused elements in the dense array, return last
			int maxId = MaxId;
			if (count < maxId)
			{
				return Dense[count];
			}

			MaxId += 1;
			AssignIndex(maxId, count);

			return maxId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Delete(int id)
		{
			// If element is not alive, nothing to be done
			if (!TryGetDense(id, out var dense))
			{
				return;
			}

			int count = AliveCount;
			AliveCount -= 1;

			// If dense is the last used element, decreasing alive count is enough
			if (dense == count - 1)
			{
				return;
			}

			// Swap dense with last element
			int lastDense = count - 1;
			AssignIndex(Dense[lastDense], dense);
			AssignIndex(id, lastDense);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CreateMany(int amount, [MaybeNull] Action<int> action = null)
		{
			int needToCreate = amount;
			if (needToCreate >= CanCreateAmount)
			{
				throw new InvalidOperationException($"Exceeded limit of ids! CanCreate: {CanCreateAmount}.");
			}

			while (AliveCount < MaxId && needToCreate > 0)
			{
				int count = AliveCount;
				AliveCount += 1;
				action?.Invoke(Dense[count]);
				needToCreate -= 1;
			}

			for (int i = 0; i < needToCreate; i++)
			{
				int count = AliveCount;
				int maxId = MaxId;
				AliveCount += 1;
				MaxId += 1;
				AssignIndex(maxId, count);
				action?.Invoke(maxId);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetDense(int id, out int dense)
		{
			if (id < 0 || id >= MaxId)
			{
				dense = default;
				return false;
			}

			dense = Sparse[id];

			return dense < AliveCount && Dense[dense] == id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			if (id < 0 || id >= MaxId)
			{
				return false;
			}

			int dense = Sparse[id];

			return dense < AliveCount && Dense[dense] == id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssignIndex(int id, int dense)
		{
			Sparse[id] = dense;
			Dense[dense] = id;
		}
	}
}