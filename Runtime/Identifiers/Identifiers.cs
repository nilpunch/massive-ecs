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
		private Identifier[] _dense;
		private int[] _sparse;

		protected Identifier[] Dense => _dense;
		protected int[] Sparse => _sparse;

		protected int AliveCount { get; set; }
		protected int MaxId { get; set; }

		public Identifiers(int dataCapacity = Constants.DataCapacity)
		{
			_dense = new Identifier[dataCapacity];
			_sparse = new int[dataCapacity];
		}

		public int CanCreateAmount => Dense.Length - AliveCount;

		public ReadOnlySpan<Identifier> AliveIdentifiers => new ReadOnlySpan<Identifier>(Dense, 0, AliveCount);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Identifier Create()
		{
			int count = AliveCount;
			AliveCount += 1;

			if (count == Dense.Length)
			{
				GrowCapacity(count + 1);
			}

			// If there are unused elements in the dense array, return last
			int maxId = MaxId;
			if (count < maxId)
			{
				return Dense[count];
			}

			var newId = new Identifier(maxId, 0);

			MaxId += 1;
			AssignIndex(newId, count);

			return newId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Delete(Identifier identifier)
		{
			// If element is not alive, nothing to be done
			if (identifier.Id < 0 || identifier.Id >= MaxId)
			{
				return;
			}
			var dense = Sparse[identifier.Id];
			if (dense >= AliveCount || Dense[dense] != identifier)
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
			AssignIndex(Identifier.IncreaseGeneration(identifier), lastDense);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Delete(int id)
		{
			// If element is not alive, nothing to be done
			if (id < 0 || id >= MaxId)
			{
				return;
			}
			var dense = Sparse[id];
			if (dense >= AliveCount)
			{
				return;
			}
			var identifier = Dense[dense];
			if (identifier.Id != id)
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
			AssignIndex(Identifier.IncreaseGeneration(identifier), lastDense);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CreateMany(int amount, [MaybeNull] Action<Identifier> action = null)
		{
			int needToCreate = amount;
			if (needToCreate + AliveCount >= Dense.Length)
			{
				GrowCapacity(needToCreate + AliveCount + 1);
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
				var newId = new Identifier(maxId, 0);
				AliveCount += 1;
				MaxId += 1;
				AssignIndex(newId, count);
				action?.Invoke(newId);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Identifier GetIdentifier(int sparseId)
		{
			return Dense[sparseId];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(Identifier identifier)
		{
			if (identifier.Id < 0 || identifier.Id >= MaxId)
			{
				return false;
			}

			int dense = Sparse[identifier.Id];

			return dense < AliveCount && Dense[dense] == identifier;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			if (id < 0 || id >= MaxId)
			{
				return false;
			}

			int dense = Sparse[id];

			return dense < AliveCount && Dense[dense].Id == id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void ResizeDense(int capacity)
		{
			Array.Resize(ref _dense, capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void ResizeSparse(int capacity)
		{
			Array.Resize(ref _sparse, capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssignIndex(Identifier identifier, int dense)
		{
			Sparse[identifier.Id] = dense;
			Dense[dense] = identifier;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void GrowCapacity(int desiredCapacity)
		{
			int newCapacity = MathHelpers.GetNextPowerOf2(desiredCapacity);
			ResizeDense(newCapacity);
			ResizeSparse(newCapacity);
		}
	}
}