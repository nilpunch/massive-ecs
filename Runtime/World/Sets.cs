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
	public class Sets
	{
		private Dictionary<string, SparseSet> SetsByIdentifiers { get; } = new Dictionary<string, SparseSet>();

		private FastList<int> Hashes { get; } = new FastList<int>();

		private FastList<string> Identifiers { get; } = new FastList<string>();
		private FastList<string> NegativeIdentifiers { get; } = new FastList<string>();

		private FastList<SetCloner> Cloners { get; } = new FastList<SetCloner>();

		public SparseSetList AllSets { get; } = new SparseSetList();

		public SparseSetList AllNegativeSets { get; } = new SparseSetList();

		public SparseSet[] Lookup { get; private set; } = Array.Empty<SparseSet>();

		public int LookupCapacity { get; private set; }

		public SetFactory SetFactory { get; }

		public Sets(SetFactory setFactory)
		{
			SetFactory = setFactory;
		}

		public event Action<(SparseSet Positive, SparseSet Negative)> SetPairCreated;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet GetExisting(string setId)
		{
			if (SetsByIdentifiers.TryGetValue(setId, out var set))
			{
				return set;
			}

			return null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet Get<T>()
		{
			var info = TypeId<T>.Info;

			EnsureLookupAt(info.Index);
			var candidate = Lookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			var (set, cloner) = SetFactory.CreateAppropriateSet<T>();

			Insert(info.FullName, set, cloner);
			Lookup[info.Index] = set;

			TryCreatePairedSet(set, info);

			return set;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet GetReflected(Type setType)
		{
			if (TypeId.TryGetInfo(setType, out var info))
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
			return (SparseSet)genericMethod?.Invoke(this, new object[] { });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureLookupAt(int index)
		{
			if (index >= LookupCapacity)
			{
				LookupCapacity = MathUtils.NextPowerOf2(index + 1);
				Lookup = Lookup.Resize(LookupCapacity);
			}
		}

		public void TryCreatePairedSet(SparseSet set, TypeIdInfo typeInfo)
		{
			var type = typeInfo.Type;

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Negative<>)
				&& type.GetGenericArguments()[0].IsDefined(typeof(StoreNegativeAttribute), false))
			{
				var itemIndex = NegativeIdentifiers.BinarySearch(typeInfo.FullName);
				if (itemIndex >= 0)
				{
					return;
				}
				var insertionIndex = ~itemIndex;
				NegativeIdentifiers.Insert(insertionIndex, typeInfo.FullName);
				AllNegativeSets.Insert(insertionIndex, set);

				var positiveSet = GetReflected(type.GetGenericArguments()[0]);
				SetPairCreated?.Invoke((positiveSet, set));
			}
			else if (type.IsDefined(typeof(StoreNegativeAttribute), false))
			{
				var negativeType = typeof(Negative<>).MakeGenericType(type);
				var negativeInfo = TypeId.GetInfo(negativeType);

				var itemIndex = NegativeIdentifiers.BinarySearch(negativeInfo.FullName);
				if (itemIndex >= 0)
				{
					return;
				}
				var insertionIndex = ~itemIndex;
				NegativeIdentifiers.Insert(insertionIndex, negativeInfo.FullName);

				var negativeSet = GetReflected(negativeType);
				AllNegativeSets.Insert(insertionIndex, negativeSet);
				SetPairCreated?.Invoke((set, negativeSet));
			}
		}

		public void Insert(string setId, SparseSet set, SetCloner cloner)
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
		public int IndexOf(SparseSet sparseSet)
		{
			return Array.IndexOf(Lookup, sparseSet);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Type TypeOf(SparseSet sparseSet)
		{
			return TypeId.GetTypeByIndex(IndexOf(sparseSet));
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
