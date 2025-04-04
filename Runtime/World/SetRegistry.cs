using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class SetRegistry
	{
		private readonly FastList<string> _setIds = new FastList<string>();
		private readonly FastList<int> _setHashes = new FastList<int>();
		private readonly FastList<SetCloner> _setCloners = new FastList<SetCloner>();
		private readonly FastListSparseSet _sets = new FastListSparseSet();

		public SparseSet[] Lookup { get; private set; } = Array.Empty<SparseSet>();

		public ISetFactory SetFactory { get; }

		public SetRegistry(ISetFactory setFactory)
		{
			SetFactory = setFactory;
		}

		public FastListSparseSet All
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _sets;
		}

		public FastList<SetCloner> Cloners
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _setCloners;
		}

		public FastList<int> Hashes
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _setHashes;
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
			var setIndex = _setIds.BinarySearch(setId);

			if (setIndex < 0)
			{
				return null;
			}

			return _sets[setIndex];
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
			return (SparseSet)genericMethod?.Invoke(this, new object[] {});
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
			var itemIndex = _setIds.BinarySearch(setId);
			if (itemIndex >= 0)
			{
				_sets[itemIndex] = set;
				_setCloners[itemIndex] = cloner;
			}
			else
			{
				var insertionIndex = ~itemIndex;
				_setIds.Insert(insertionIndex, setId);
				_setHashes.Insert(insertionIndex, setId.GetHashCode());
				_sets.Insert(insertionIndex, set);
				_setCloners.Insert(insertionIndex, cloner);
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
