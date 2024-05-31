using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class FilterRegistry
	{
		private readonly GenericLookup<IFilter> _filterLookup = new GenericLookup<IFilter>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IFilter Get<TInclude, TExclude>(SetRegistry setRegistry)
			where TInclude : struct, IIncludeSelector
			where TExclude : struct, IExcludeSelector
		{
			var filter = _filterLookup.GetOrDefault<Tuple<TInclude, TExclude>>();

			if (filter == null)
			{
				var include = default(TInclude).SelectReadOnly(setRegistry);
				var exclude = default(TExclude).SelectReadOnly(setRegistry);

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
