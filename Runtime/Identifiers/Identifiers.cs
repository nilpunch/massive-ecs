using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class Identifiers
	{
		public int[] Ids { get; }
		public int MaxId { get; set; }

		// Recycling
		public int Available { get; set; }
		public int Next { get; set; }

		public Identifiers(int dataCapacity = Constants.DataCapacity)
		{
			Ids = new int[dataCapacity];
			Next = dataCapacity;
		}

		public int CanCreateAmount => Ids.Length - MaxId + Available;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Create()
		{
			if (Available > 0)
			{
				var nextId = Next;
				(Next, Ids[nextId]) = (Ids[nextId], nextId);
				Available -= 1;
				return nextId;
			}

			int maxId = MaxId;
			if (MaxId >= Ids.Length + Available)
			{
				throw new InvalidOperationException($"Exceeded limit of ids! Limit: {Ids.Length}.");
			}

			MaxId += 1;
			Ids[maxId] = maxId;
			return maxId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Delete(int id)
		{
			if (!IsAlive(id))
			{
				return;
			}

			(Next, Ids[id]) = (id, Next);
			Available += 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CreateMany(int amount, [MaybeNull] Action<int> action = null)
		{
			int needToCreate = amount;
			if (needToCreate >= CanCreateAmount)
			{
				throw new InvalidOperationException($"Exceeded limit of ids! CanCreate: {CanCreateAmount}.");
			}

			while (Available > 0)
			{
				var nextId = Next;
				(Next, Ids[nextId]) = (Ids[nextId], nextId);
				Available -= 1;

				action?.Invoke(nextId);
				needToCreate -= 1;
			}

			for (int i = 0; i < needToCreate; i++)
			{
				int maxId = MaxId;
				MaxId += 1;
				Ids[maxId] = maxId;

				action?.Invoke(maxId);
			}
		}

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			return id < MaxId && Ids[id] == id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator() => new Enumerator(Ids, MaxId);

		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
		[Il2CppSetOption(Option.DivideByZeroChecks, false)]
		public struct Enumerator
		{
			private readonly int[] _ids;
			private readonly int _length;
			private int _currentIndex;

			public Enumerator(int[] ids, int length)
			{
				_ids = ids;
				_length = length;
				_currentIndex = -1;
			}

			public bool MoveNext()
			{
				while (++_currentIndex < _length && _ids[_currentIndex] != _currentIndex)
				{
				}

				return _currentIndex < _length;
			}

			public void Reset() => _currentIndex = -1;

			public int Current => _ids[_currentIndex];
		}
	}
}