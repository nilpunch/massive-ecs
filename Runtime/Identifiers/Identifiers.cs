using System;
using System.Runtime.CompilerServices;

namespace Massive.Plugins.massive.Runtime.Identifiers
{
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Create()
		{
			int maxId = MaxId;

			if (maxId >= Ids.Length)
			{
				throw new InvalidOperationException($"Exceeded limit of ids! Limit: {Ids.Length}.");
			}

			if (Available == 0)
			{
				MaxId += 1;
				Ids[maxId] = maxId;
				return maxId;
			}
			else
			{
				var recycledId = Next;
				Next = Ids[Next];
				Available -= 1;
				return recycledId;
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