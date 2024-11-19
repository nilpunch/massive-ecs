using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class FilterRegistry
	{
		private readonly GenericLookup<Filter> _filterLookup = new GenericLookup<Filter>();
		private readonly SetRegistry _setRegistry;

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

			if (filter.IsValid())
			{
				var included = new TInclude().Select(_setRegistry);
				var excluded = new TExclude().Select(_setRegistry);

				filter = included.Length != 0 || excluded.Length != 0
					? new Filter(included, excluded)
					: Filter.Empty;

				_filterLookup.Assign<Tuple<TInclude, TExclude>>(filter);
			}

			return filter;
		}
	}
}
