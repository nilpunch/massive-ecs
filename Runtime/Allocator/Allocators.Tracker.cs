#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public partial class Allocators
	{
		public Allocation[] Allocations { get; private set; } = Array.Empty<Allocation>();
		private int AllocationsCapacity { get; set; }
		public int UsedAllocations { get; set; }

		public int NextFreeAllocation { get; set; } = Constants.InvalidId;

		public int[] Heads { get; private set; } = Array.Empty<int>();
		private int HeadCapacity { get; set; }
		public int UsedHeads { get; set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void TrackAllocation(int id, AllocatorChunkId allocatorChunkId)
		{
			TrackAllocation(id, allocatorChunkId.ChunkId, allocatorChunkId.AllocatorId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void TrackAllocation(int id, ChunkId chunkId, int allocatorId)
		{
			EnsureTrackerHeadAt(id);

			ref var head = ref Heads[id];

			int index;
			if (NextFreeAllocation != Constants.InvalidId)
			{
				index = NextFreeAllocation;
				NextFreeAllocation = Allocations[index].NextAllocation;
			}
			else
			{
				index = UsedAllocations++;
				EnsureTrackerAllocationAt(index);
			}

			ref var allocation = ref Allocations[index];
			allocation.ChunkId = chunkId;
			allocation.NextAllocation = head;
			allocation.AllocatorId = allocatorId;

			head = index;

			UsedHeads = MathUtils.Max(UsedHeads, id + 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free(int id)
		{
			if (id >= HeadCapacity || Heads[id] == Constants.InvalidId)
			{
				return;
			}

			var index = Heads[id];
			while (index != Constants.InvalidId)
			{
				ref var allocation = ref Allocations[index];
				Lookup[allocation.AllocatorId].TryFree(allocation.ChunkId);

				var next = allocation.NextAllocation;
				allocation.NextAllocation = NextFreeAllocation;
				NextFreeAllocation = index;

				index = next;
			}

			Heads[id] = Constants.InvalidId;
		}

		/// <summary>
		/// Sets the current state for serialization or rollback purposes.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetTrackerState(int usedAllocations, int nextFreeAllocation, int usedHeads)
		{
			UsedAllocations = usedAllocations;
			NextFreeAllocation = nextFreeAllocation;
			UsedHeads = usedHeads;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTrackerTo(Allocators other)
		{
			other.EnsureTrackerAllocationAt(UsedAllocations - 1);
			other.EnsureTrackerHeadAt(UsedHeads - 1);

			Array.Copy(Allocations, other.Allocations, UsedAllocations);
			Array.Copy(Heads, other.Heads, UsedHeads);

			if (UsedHeads < other.UsedHeads)
			{
				Array.Fill(other.Heads, Constants.InvalidId, UsedHeads, other.UsedHeads - UsedHeads);
			}

			other.SetTrackerState(UsedAllocations, NextFreeAllocation, UsedHeads);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureTrackerHeadAt(int index)
		{
			if (index >= HeadCapacity)
			{
				var newCapacity = MathUtils.NextPowerOf2(index + 1);

				Heads = Heads.Resize(newCapacity);
				Array.Fill(Heads, Constants.InvalidId, HeadCapacity, newCapacity - HeadCapacity);
				HeadCapacity = newCapacity;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureTrackerAllocationAt(int index)
		{
			if (index >= AllocationsCapacity)
			{
				AllocationsCapacity = MathUtils.NextPowerOf2(index + 1);
				Allocations = Allocations.Resize(AllocationsCapacity);
			}
		}

		public struct Allocation
		{
			public ChunkId ChunkId;

			/// <summary>
			/// Non-deterministic, used for lookups.<br/>
			/// Don't store it in simulation.
			/// </summary>
			public int AllocatorId;

			/// <summary>
			/// Next free or next allocation in list.
			/// </summary>
			public int NextAllocation;
		}
	}
}
