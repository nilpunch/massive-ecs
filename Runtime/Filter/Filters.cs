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
	public class Filters
	{
		private Sets Sets { get; }
		private SetComparer Comparer { get; }

		private Dictionary<int, Filter> CombinationLookup { get; } = new Dictionary<int, Filter>();

		public Filter[] Lookup { get; private set; } = Array.Empty<Filter>();

		public Filter Empty { get; } = new Filter();

		public Filters(Sets sets)
		{
			Sets = sets;
			Comparer = new SetComparer(Sets);
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

			var included = new TInclude().Select(Sets);
			var excluded = new TExclude().Select(Sets);

			var filter = Get(included, excluded);

			Lookup[info.Index] = filter;

			return filter;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Filter Get(SparseSet[] included = null, SparseSet[] excluded = null)
		{
			included ??= Array.Empty<SparseSet>();
			excluded ??= Array.Empty<SparseSet>();

			ConflictingFilterException.ThrowIfHasConflicts(included, excluded);
			ConflictingFilterException.ThrowIfHasDuplicates(included, ConflictingFilterException.FilterType.Include);
			ConflictingFilterException.ThrowIfHasDuplicates(excluded, ConflictingFilterException.FilterType.Exclude);

			Array.Sort(included, Comparer.ByIndex);
			Array.Sort(excluded, Comparer.ByIndex);

			var includeCode = SetUtils.GetOrderedHashCode(included, Sets);
			var excludeCode = SetUtils.GetOrderedHashCode(excluded, Sets);
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
			private readonly Sets _sets;

			public readonly Comparison<SparseSet> ByIndex;

			public SetComparer(Sets sets)
			{
				_sets = sets;
				ByIndex = Compare;
			}

			private int Compare(SparseSet a, SparseSet b)
			{
				return _sets.IndexOf(a).CompareTo(_sets.IndexOf(b));
			}
		}
	}
}
