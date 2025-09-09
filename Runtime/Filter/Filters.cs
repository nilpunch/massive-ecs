#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class Filters
	{
		private BitSets BitSets { get; }
		private SetComparer Comparer { get; }

		private Dictionary<int, Filter> CombinationLookup { get; } = new Dictionary<int, Filter>();

		public Filter[] Lookup { get; private set; } = Array.Empty<Filter>();

		public Filter Empty { get; }

		public Filters(BitSets bitSets)
		{
			BitSets = bitSets;
			Comparer = new SetComparer(BitSets);
			Empty = new Filter();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Filter Get<TInclude, TExclude>()
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			var info = TypeId<Tuple<TInclude, TExclude>>.Info;

			EnsureLookupAt(info.Index);
			var candidate = Lookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			var included = new TInclude().Select(BitSets);
			var excluded = new TExclude().Select(BitSets);

			var filter = Get(included, excluded);

			Lookup[info.Index] = filter;

			return filter;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Filter Get(BitSet[] included = null, BitSet[] excluded = null)
		{
			included ??= Array.Empty<BitSet>();
			excluded ??= Array.Empty<BitSet>();

			ConflictingFilterException.ThrowIfHasConflicts(included, excluded);
			ConflictingFilterException.ThrowIfHasDuplicates(included, ConflictingFilterException.FilterType.Include);
			ConflictingFilterException.ThrowIfHasDuplicates(excluded, ConflictingFilterException.FilterType.Exclude);

			Array.Sort(included, Comparer.ByIndex);
			Array.Sort(excluded, Comparer.ByIndex);

			var includeCode = BitSets.GetOrderedHashCode(included);
			var excludeCode = BitSets.GetOrderedHashCode(excluded);
			var fullCode = MathUtils.CombineHashes(includeCode, excludeCode);

			if (CombinationLookup.TryGetValue(fullCode, out var filter))
			{
				return filter;
			}

			filter = included.Length != 0 || excluded.Length != 0
				? new Filter(included, excluded)
				: Empty;
			CombinationLookup.Add(fullCode, filter);
			return filter;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureLookupAt(int index)
		{
			if (index >= Lookup.Length)
			{
				Lookup = Lookup.Resize(MathUtils.NextPowerOf2(index + 1));
			}
		}

		private class SetComparer
		{
			private readonly BitSets _bitSets;

			public readonly Comparison<BitSet> ByIndex;

			public SetComparer(BitSets bitSets)
			{
				_bitSets = bitSets;
				ByIndex = Compare;
			}

			private int Compare(BitSet a, BitSet b)
			{
				return _bitSets.IndexOf(a).CompareTo(_bitSets.IndexOf(b));
			}
		}
	}
}
