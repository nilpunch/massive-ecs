using System;
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

		public ReadOnlySpan<int> UsedIds => new ReadOnlySpan<int>(Ids, 0, MaxId);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Create()
		{
			if (Available == 0)
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
			else
			{
				var nextId = Next;
				(Next, Ids[nextId]) = (Ids[nextId], nextId);
				Available -= 1;
				return nextId;
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
	}
}