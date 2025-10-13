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
	public partial class Sets
	{
		private Dictionary<string, BitSet> SetsByIdentifiers { get; } = new Dictionary<string, BitSet>();

		private FastList<int> Hashes { get; } = new FastList<int>();

		private FastList<string> Identifiers { get; } = new FastList<string>();

		private FastList<SetCloner> Cloners { get; } = new FastList<SetCloner>();

		public BitSetList AllSets { get; } = new BitSetList();

		public BitSet[] Lookup { get; private set; } = Array.Empty<BitSet>();

		public int LookupCapacity { get; private set; }

		public SetFactory SetFactory { get; }

		public Components Components { get; }

		public Sets(SetFactory setFactory, Components components)
		{
			SetFactory = setFactory;
			Components = components;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet GetExisting(string setId)
		{
			if (SetsByIdentifiers.TryGetValue(setId, out var set))
			{
				return set;
			}

			return null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet Get<T>()
		{
			var info = ComponentId<T>.Info;

			EnsureLookupAt(info.Index);
			var candidate = Lookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			var (set, cloner) = SetFactory.CreateAppropriateSet<T>();

			set.ComponentId = info.Index;
			set.ComponentIndex = set.ComponentId >> 6;
			set.ComponentMask = 1UL << (set.ComponentId & 63);
			set.ComponentMaskNegative = ~set.ComponentMask;
			set.Components = Components;

			InsertSet(info.FullName, set, cloner);
			Lookup[info.Index] = set;

			return set;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet GetReflected(Type setType)
		{
			if (ComponentId.TryGetInfo(setType, out var info))
			{
				EnsureLookupAt(info.Index);
				var candidate = Lookup[info.Index];

				if (candidate != null)
				{
					return candidate;
				}
			}

			var createMethod = typeof(Sets).GetMethod(nameof(Get));
			var genericMethod = createMethod?.MakeGenericMethod(setType);
			return (BitSet)genericMethod?.Invoke(this, new object[] { });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureLookupAt(int index)
		{
			if (index >= LookupCapacity)
			{
				Lookup = Lookup.ResizeToNextPowOf2(index + 1);
				LookupCapacity = Lookup.Length;
				Components.EnsureComponentsCapacity(LookupCapacity);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void InsertSet(string setId, BitSet set, SetCloner cloner)
		{
			// Maintain items sorted.
			var itemIndex = Identifiers.BinarySearch(setId);
			if (itemIndex >= 0)
			{
				MassiveException.Throw($"You are trying to insert already existing item:{setId}.");
			}
			else
			{
				var insertionIndex = ~itemIndex;
				Identifiers.Insert(insertionIndex, setId);
				AllSets.Insert(insertionIndex, set);
				Cloners.Insert(insertionIndex, cloner);
				Hashes.Insert(insertionIndex, setId.GetHashCode());
				SetsByIdentifiers.Add(setId, set);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(BitSet bitSet)
		{
			return bitSet.ComponentId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Type TypeOf(BitSet bitSet)
		{
			return ComponentId.GetTypeByIndex(bitSet.ComponentId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetOrderedHashCode(BitSet[] orderedSets)
		{
			var hash = 17;
			for (var i = 0; i < orderedSets.Length; i++)
			{
				var index = IndexOf(orderedSets[i]) + 1; // Avoid zero.
				hash = unchecked(hash * 31 + index);
			}

			return hash;
		}

		/// <summary>
		/// Copies all sets from this registry into the specified one.
		/// Clears sets in the target registry that are not present in the source.
		/// </summary>
		/// <remarks>
		/// Throws if the set factories are incompatible.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(Sets other)
		{
			IncompatibleConfigsException.ThrowIfIncompatible(SetFactory, other.SetFactory);

			// Copy present sets.
			foreach (var cloner in Cloners)
			{
				cloner.CopyTo(other);
			}

			// Clear other sets.
			var hashes = Hashes;
			var otherHashes = other.Hashes;
			var otherSets = other.AllSets;

			if (hashes.Count == otherHashes.Count)
			{
				// Skip clearing if target has exactly the same sets.
				return;
			}

			var index = 0;
			for (var otherIndex = 0; otherIndex < otherSets.Count; otherIndex++)
			{
				if (index >= hashes.Count || otherHashes[otherIndex] != hashes[index])
				{
					otherSets[otherIndex].ClearWithoutNotify();
				}
				else
				{
					index++;
				}
			}
		}
	}
}
