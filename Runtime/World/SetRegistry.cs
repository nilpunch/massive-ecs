using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class SetRegistry
	{
		public Dictionary<string, SparseSet> SetsByIdentifiers { get; } = new Dictionary<string, SparseSet>();

		public FastList<string> Identifiers { get; } = new FastList<string>();

		public FastList<int> Hashes { get; } = new FastList<int>();

		public FastList<SetCloner> Cloners { get; } = new FastList<SetCloner>();

		public FastListSparseSet AllSets { get; } = new FastListSparseSet();

		public SparseSet[] Lookup { get; private set; } = Array.Empty<SparseSet>();

		public SetFactory SetFactory { get; }

		public SetRegistry(SetFactory setFactory)
		{
			SetFactory = setFactory;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet GetExisting(int typeIndex)
		{
			if (typeIndex >= Lookup.Length)
			{
				return null;
			}

			return Lookup[typeIndex];
		}

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
		public SparseSet Get<TKey>()
		{
			var info = TypeId<TKey>.Info;

			EnsureLookupAt(info.Index);
			var candidate = Lookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			var (set, cloner) = SetFactory.CreateAppropriateSet<TKey>();

			Insert(info.FullName, set, cloner);
			Lookup[info.Index] = set;

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

			var createMethod = typeof(SetRegistry).GetMethod(nameof(Get));
			var genericMethod = createMethod?.MakeGenericMethod(setType);
			return (SparseSet)genericMethod?.Invoke(this, new object[] { });
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
		public void Insert(string setId, SparseSet set, SetCloner cloner)
		{
			// Maintain items sorted.
			var itemIndex = Identifiers.BinarySearch(setId);
			if (itemIndex >= 0)
			{
				throw new Exception("Trying to insert already existing item.");
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
	}
}
