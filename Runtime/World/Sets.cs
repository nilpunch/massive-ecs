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
			var info = TypeId<SetKind, T>.Info;

			EnsureLookupByTypeAt(info.Index);
			var candidate = LookupByTypeId[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			var (set, cloner) = SetFactory.CreateAppropriateSet<T>();

			InsertSet(info.FullName, set, cloner);
			LookupByTypeId[info.Index] = set;

			set.SetupComponent(this, Components, info.Index);

			return set;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet GetReflected(Type setType)
		{
			if (TypeId<SetKind>.TryGetInfo(setType, out var info))
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureBinded(BitSet set)
		{
			if (set.IsComponentBound)
			{
				return;
			}

			var componentId = ComponentCount++;
			EnsureLookupByComponentAt(componentId);
			set.BindComponentId(componentId);
			LookupByComponentId[componentId] = set;
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
			return bitSet.TypeId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Type TypeOf(BitSet bitSet)
		{
			return TypeId<SetKind>.GetTypeByIndex(bitSet.TypeId);
		}

		public void Reset()
		{
			for (var i = 0; i < ComponentCount; i++)
			{
				ref var set = ref LookupByComponentId[i];
				set.UnbindComponentId();
				set.Reset();
				set = null;
			}

			ComponentCount = 0;
		}

		/// <summary>
		/// Copies all sets from this registry into the specified one.
		/// Clears sets in the target registry that are not present in the source.
		/// </summary>
		/// <remarks>
		/// Throws if the set factories are incompatible.
		/// </remarks>
		public void CopyTo(Sets other)
		{
			IncompatibleConfigsException.ThrowIfIncompatible(SetFactory, other.SetFactory);

			// Copy present sets.
			foreach (var cloner in Cloners)
			{
				cloner.CopyTo(other);
			}

			other.EnsureLookupByComponentAt(ComponentCount - 1);

			// Reorder the target world's component IDs to match the current world's layout.
			// This ensures that both worlds use identical component indices for their masks.
			for (var i = 0; i < ComponentCount; i++)
			{
				var set = LookupByComponentId[i];
				ref var otherSet = ref other.LookupByComponentId[i];

				if (otherSet == null || otherSet.TypeId != set.TypeId)
				{
					ref var otherMatchedSet = ref other.LookupByComponentId[other.LookupByTypeId[set.TypeId].ComponentId];
					(otherSet, otherMatchedSet) = (otherMatchedSet, otherSet);
				}

				otherSet.BindComponentId(i);
			}

			// Ubind other sets and reset them.
			for (var i = ComponentCount; i < other.ComponentCount; i++)
			{
				ref var otherSet = ref other.LookupByComponentId[i];
				otherSet.UnbindComponentId();
				otherSet.Reset();
				otherSet = null;
			}

			other.ComponentCount = ComponentCount;
		}
	}

	internal struct SetKind
	{
	}
}
