﻿#if !MASSIVE_DISABLE_ASSERT
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
		private Dictionary<string, BitSet> SetsByNames { get; } = new Dictionary<string, BitSet>();

		private FastList<string> Names { get; } = new FastList<string>();

		private FastList<SetCloner> Cloners { get; } = new FastList<SetCloner>();

		public BitSetList Sorted { get; } = new BitSetList();

		public BitSet[] LookupByTypeId { get; private set; } = Array.Empty<BitSet>();

		public BitSet[] LookupByComponentId { get; private set; } = Array.Empty<BitSet>();

		public int LookupCapacity { get; private set; }

		public int ComponentCount { get; private set; }

		private SetFactory SetFactory { get; }

		private Components Components { get; }

		public Sets(SetFactory setFactory, Components components)
		{
			SetFactory = setFactory;
			Components = components;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet GetExisting(string setId)
		{
			if (SetsByNames.TryGetValue(setId, out var set))
			{
				return set;
			}

			return null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet Get<T>()
		{
			var info = TypeId<T>.Info;

			EnsureLookupByTypeAt(info.Index);
			var candidate = LookupByTypeId[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			var (set, cloner) = SetFactory.CreateAppropriateSet<T>();

			InsertSet(info.FullName, set, cloner);
			LookupByTypeId[info.Index] = set;

			var componentId = ComponentCount++;
			EnsureLookupByComponentAt(componentId);
			set.SetupComponent(Components, info.Index, componentId);
			LookupByComponentId[componentId] = set;

			return set;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet GetReflected(Type setType)
		{
			if (TypeId.TryGetInfo(setType, out var info))
			{
				EnsureLookupByTypeAt(info.Index);
				var candidate = LookupByTypeId[info.Index];

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
		public void EnsureLookupByTypeAt(int index)
		{
			if (index >= LookupCapacity)
			{
				LookupByTypeId = LookupByTypeId.ResizeToNextPowOf2(index + 1);
				LookupCapacity = LookupByTypeId.Length;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureLookupByComponentAt(int index)
		{
			if (index >= LookupByComponentId.Length)
			{
				LookupByComponentId = LookupByComponentId.ResizeToNextPowOf2(index + 1);
				Components.EnsureComponentsCapacity(LookupByComponentId.Length);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void InsertSet(string setName, BitSet set, SetCloner cloner)
		{
			// Maintain items sorted.
			var itemIndex = Names.BinarySearch(setName);
			if (itemIndex >= 0)
			{
				MassiveException.Throw($"You are trying to insert already existing item:{setName}.");
			}
			else
			{
				var insertionIndex = ~itemIndex;
				Names.Insert(insertionIndex, setName);
				Sorted.Insert(insertionIndex, set);
				Cloners.Insert(insertionIndex, cloner);
				SetsByNames.Add(setName, set);
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
			return TypeId.GetTypeByIndex(bitSet.ComponentId);
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

			other.EnsureLookupByComponentAt(ComponentCount - 1);

			// Sort lookup to match Components.
			for (var i = 0; i < ComponentCount; i++)
			{
				var set = LookupByComponentId[i];
				ref var otherSet = ref other.LookupByComponentId[i];

				if (otherSet == null || otherSet.TypeId != set.TypeId)
				{
					ref var otherMatchedSet = ref other.LookupByComponentId[other.LookupByTypeId[set.TypeId].ComponentId];
					(otherSet, otherMatchedSet) = (otherMatchedSet, otherSet);
				}

				otherSet.SetComponentId(i);
			}

			// Clear other sets.
			for (var i = ComponentCount; i < other.ComponentCount; i++)
			{
				var otherSet = other.LookupByComponentId[i];
				otherSet.SetComponentId(i);
				otherSet.ClearWithoutNotify();
			}
		}
	}
}
