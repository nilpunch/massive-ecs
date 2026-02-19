#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using Preserve = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute;
using Member = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public partial class Sets
	{
		public BitSet[] LookupByTypeId { get; private set; } = Array.Empty<BitSet>();

		public SetCloner[] ClonerByTypeId { get; private set; } = Array.Empty<SetCloner>();

		public BitSet[] LookupByComponentId { get; private set; } = Array.Empty<BitSet>();

		public SetCloner[] ClonerByComponentId { get; private set; } = Array.Empty<SetCloner>();

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
		public BitSet Get<[Preserve(Member.PublicFields | Member.NonPublicFields | Member.Interfaces)] T>()
		{
			var info = TypeId<SetKind, T>.Info;

			EnsureLookupByTypeAt(info.Index);
			var candidate = LookupByTypeId[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			var (set, cloner) = SetFactory.CreateAppropriateSet<T>();

			LookupByTypeId[info.Index] = set;
			ClonerByTypeId[info.Index] = cloner;

			set.SetupComponent(this, Components, info.Index);

			return set;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet GetReflected([Preserve(Member.PublicFields | Member.NonPublicFields | Member.Interfaces)] Type setType)
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
			var genericMethod = createMethod.MakeGenericMethod(setType);
			return (BitSet)genericMethod.Invoke(this, new object[] { });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureLookupByTypeAt(int index)
		{
			if (index >= LookupCapacity)
			{
				LookupByTypeId = LookupByTypeId.ResizeToNextPowOf2(index + 1);
				ClonerByTypeId = ClonerByTypeId.Resize(LookupByTypeId.Length);
				LookupCapacity = LookupByTypeId.Length;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureLookupByComponentAt(int index)
		{
			if (index >= LookupByComponentId.Length)
			{
				LookupByComponentId = LookupByComponentId.ResizeToNextPowOf2(index + 1);
				ClonerByComponentId = ClonerByComponentId.Resize(LookupByComponentId.Length);
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
			ClonerByComponentId[componentId] = ClonerByTypeId[set.TypeId];
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
				ClonerByComponentId[i] = null;
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
			for (var i = 0; i < ComponentCount; i++)
			{
				ClonerByComponentId[i].CopyTo(other);
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
					var matchedComponentId = other.LookupByTypeId[set.TypeId].ComponentId;
					ref var otherMatchedSet = ref other.LookupByComponentId[matchedComponentId];
					(otherSet, otherMatchedSet) = (otherMatchedSet, otherSet);

					ref var otherCloner = ref other.ClonerByComponentId[i];
					ref var otherMatchedCloner = ref other.ClonerByComponentId[matchedComponentId];
					(otherCloner, otherMatchedCloner) = (otherMatchedCloner, otherCloner);
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
				other.ClonerByComponentId[i] = null;
			}

			other.ComponentCount = ComponentCount;
		}
	}

	internal struct SetKind
	{
	}
}
