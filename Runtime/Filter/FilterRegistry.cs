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
	public class FilterRegistry
	{
		private readonly GenericLookup<Filter> _filterLookup = new GenericLookup<Filter>();
		private readonly Dictionary<int, Filter> _codeLookup = new Dictionary<int, Filter>();
		private readonly SetRegistry _setRegistry;

		public Filter Empty { get; } = new Filter();

		public FilterRegistry(SetRegistry setRegistry)
		{
			_setRegistry = setRegistry;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Filter Get<TInclude, TExclude>()
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			var filter = _filterLookup.Find<Tuple<TInclude, TExclude>>();

			if (filter == null)
			{
				var included = new TInclude().Select(_setRegistry);
				var excluded = new TExclude().Select(_setRegistry);

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

			MassiveAssert.NoConflictsInFilter(included, excluded);
			MassiveAssert.ContainsDuplicates(included, "Included contains duplicate sets!");
			MassiveAssert.ContainsDuplicates(excluded, "Excluded contains duplicate sets!");

			var includeCode = SetUtils.GetUnorderedHashCode(included, _setRegistry);
			var excludeCode = SetUtils.GetUnorderedHashCode(excluded, _setRegistry);
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
	}
}
