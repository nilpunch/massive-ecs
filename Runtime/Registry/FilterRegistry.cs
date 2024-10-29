using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
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

			if (filter == null)
			{
				var include = new TInclude().Select(_setRegistry);
				var exclude = new TExclude().Select(_setRegistry);

				filter = include.Length != 0 || exclude.Length != 0
					? new Filter(include, exclude)
					: Filter.Empty;

				_filterLookup.Assign<Tuple<TInclude, TExclude>>(filter);
			}

			return filter;
		}
	}
}
