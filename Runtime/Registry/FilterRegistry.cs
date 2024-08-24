using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class FilterRegistry
	{
		private readonly GenericLookup<IFilter> _filterLookup = new GenericLookup<IFilter>();
		private readonly SetRegistry _setRegistry;

		public FilterRegistry(SetRegistry setRegistry)
		{
			_setRegistry = setRegistry;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IFilter Get<TInclude, TExclude>()
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			var filter = _filterLookup.GetOrDefault<Tuple<TInclude, TExclude>>();

			if (filter == null)
			{
				var include = new TInclude().SelectReadOnly(_setRegistry);
				var exclude = new TExclude().SelectReadOnly(_setRegistry);

				if (include.Length != 0 && exclude.Length != 0)
				{
					filter = new Filter(include, exclude);
				}
				else if (include.Length != 0)
				{
					filter = new IncludeFilter(include);
				}
				else if (exclude.Length != 0)
				{
					filter = new ExcludeFilter(exclude);
				}
				else
				{
					filter = EmptyFilter.Instance;
				}

				_filterLookup.Assign<Tuple<TInclude, TExclude>>(filter);
			}

			return filter;
		}
	}
}
