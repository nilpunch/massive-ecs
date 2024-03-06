using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class Identifiers : IEnumerable<int>
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
			else
			{
				int maxId = MaxId;

				if (maxId >= Ids.Length)
				{
					throw new InvalidOperationException($"Exceeded limit of ids! Limit: {Ids.Length}.");
				}

				MaxId += 1;
				Ids[maxId] = maxId;
				return maxId;
			}
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
		public bool IsAlive(int id)
		{
			return id < MaxId && Ids[id] == id;
		}

		IEnumerator<int> IEnumerable<int>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator() => new Enumerator(Ids, MaxId);

		public struct Enumerator : IEnumerator<int>
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
				// Iterate until we find alive entity
				while (++_currentIndex < _length && _ids[_currentIndex] != _currentIndex)
				{
				}

				return _currentIndex < _length;
			}

			public void Reset()
			{
				_currentIndex = -1;
			}

			public int Current => _ids[_currentIndex];
			
			object IEnumerator.Current => Current;

			public void Dispose() { }
		}
	}
}