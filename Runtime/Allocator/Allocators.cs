#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public partial class Allocators
	{
		private Dictionary<string, Allocator> AllocatorsByIdentifiers { get; } = new Dictionary<string, Allocator>();

		private FastList<int> Hashes { get; } = new FastList<int>();

		private FastList<string> Identifiers { get; } = new FastList<string>();

		private FastList<AllocatorCloner> Cloners { get; } = new FastList<AllocatorCloner>();

		public FastList<Allocator> AllAllocators { get; } = new FastList<Allocator>();

		public Allocator[] Lookup { get; private set; } = Array.Empty<Allocator>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Allocator GetExisting(string allocatorId)
		{
			if (AllocatorsByIdentifiers.TryGetValue(allocatorId, out var allocator))
			{
				return allocator;
			}

			return null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Allocator Get<T>() where T : unmanaged
		{
			var allocatorId = AllocatorId<T>.Index;

			EnsureLookupAt(allocatorId);
			var candidate = Lookup[allocatorId];

			if (candidate != null)
			{
				return candidate;
			}

			var allocator = new Allocator<T>(DefaultValueUtils.GetDefaultValueFor<T>());
			var cloner = new AllocatorCloner<T>(allocator);

			Insert(AllocatorId<T>.FullName, allocator, cloner);
			Lookup[allocatorId] = allocator;

			return allocator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Allocator GetReflected(Type allocatorType)
		{
			if (AllocatorId.TryGetInfo(allocatorType, out var info))
			{
				EnsureLookupAt(info.Index);
				var candidate = Lookup[info.Index];

				if (candidate != null)
				{
					return candidate;
				}
			}

			var createMethod = typeof(Allocators).GetMethod(nameof(Get));
			var genericMethod = createMethod?.MakeGenericMethod(allocatorType);
			return (Allocator)genericMethod?.Invoke(this, new object[] { });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureLookupAt(int index)
		{
			if (index >= Lookup.Length)
			{
				Lookup = Lookup.Resize(MathUtils.NextPowerOf2(index + 1));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Insert(string allocatorId, Allocator allocator, AllocatorCloner cloner)
		{
			// Maintain items sorted.
			var itemIndex = Identifiers.BinarySearch(allocatorId);
			if (itemIndex >= 0)
			{
				MassiveException.Throw($"You are trying to insert already existing item:{allocatorId}.");
			}
			else
			{
				var insertionIndex = ~itemIndex;
				Identifiers.Insert(insertionIndex, allocatorId);
				AllAllocators.Insert(insertionIndex, allocator);
				Cloners.Insert(insertionIndex, cloner);
				Hashes.Insert(insertionIndex, allocatorId.GetHashCode());
				AllocatorsByIdentifiers.Add(allocatorId, allocator);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(Allocator allocator)
		{
			return Array.IndexOf(Lookup, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Type TypeOf(Allocator allocator)
		{
			return AllocatorId.GetTypeByIndex(IndexOf(allocator));
		}

		/// <summary>
		/// Copies all allocators from this registry into the specified one.
		/// Clears allocators in the target registry that are not present in the source.
		/// </summary>
		/// <remarks>
		/// Throws if the allocator factories are incompatible.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(Allocators other)
		{
			// Copy tracker.
			CopyTrackerTo(other);

			// Copy present allocators.
			foreach (var cloner in Cloners)
			{
				cloner.CopyTo(other);
			}

			// Reset other allocators to initial state.
			var hashes = Hashes;
			var otherHashes = other.Hashes;
			var otherAllocators = other.AllAllocators;

			if (hashes.Count == otherHashes.Count)
			{
				// Skip resetting if target has exactly the same allocators.
				return;
			}

			var index = 0;
			for (var otherIndex = 0; otherIndex < otherAllocators.Count; otherIndex++)
			{
				if (index >= hashes.Count || otherHashes[otherIndex] != hashes[index])
				{
					otherAllocators[otherIndex].Reset();
				}
				else
				{
					index++;
				}
			}
		}
	}
}
