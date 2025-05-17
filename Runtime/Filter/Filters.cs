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
		private readonly GenericLookup<Filter> _filterLookup = new GenericLookup<Filter>();
		private readonly Dictionary<int, Filter> _codeLookup = new Dictionary<int, Filter>();
		private readonly Sets _sets;

		public Filter Empty { get; } = new Filter();

		public Filters(Sets sets)
		{
			_sets = sets;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Filter Get<TInclude, TExclude>()
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			var filter = _filterLookup.Find<Tuple<TInclude, TExclude>>();

			if (filter == null)
			{
				var included = new TInclude().Select(_sets);
				var excluded = new TExclude().Select(_sets);

				filter = Get(included, excluded);

				_filterLookup.Assign<Tuple<TInclude, TExclude>>(filter);
			}

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

			SparseSetComparer.Registry = _sets;
			Array.Sort(included, SparseSetComparer.ByRegistryIndex);
			Array.Sort(excluded, SparseSetComparer.ByRegistryIndex);

			var includeCode = SetUtils.GetOrderedHashCode(included, _sets);
			var excludeCode = SetUtils.GetOrderedHashCode(excluded, _sets);
			var fullCode = MathUtils.CombineHashes(includeCode, excludeCode);

			if (_codeLookup.TryGetValue(fullCode, out var filter))
			{
				return filter;
			}

			filter = included.Length != 0 || excluded.Length != 0
				? new Filter(included, excluded)
				: Empty;
			_codeLookup.Add(fullCode, filter);
			return filter;
		}

		private static class SparseSetComparer
		{
			public static Sets Registry;

			public static readonly Comparison<SparseSet> ByRegistryIndex = Compare;

			private static int Compare(SparseSet a, SparseSet b)
			{
				return Registry.IndexOf(a).CompareTo(Registry.IndexOf(b));
			}
		}
	}
}
