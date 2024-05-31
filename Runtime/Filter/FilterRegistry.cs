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
		public IFilter Get<TFilterSelector>(TFilterSelector selector)
			where TFilterSelector : struct, IFilterSelector
		{
			var filter = _filterLookup.GetOrDefault<TFilterSelector>();

			if (filter == null)
			{
				var include = new IReadOnlySet[selector.IncludeCount];
				var exclude = new IReadOnlySet[selector.ExcludeCount];
				selector.Select(include, exclude);

				if (selector.IncludeCount != 0 && selector.ExcludeCount != 0)
				{
					filter = new Filter(include, exclude);
				}
				else if (selector.IncludeCount != 0)
				{
					filter = new IncludeFilter(include);
				}
				else if (selector.ExcludeCount != 0)
				{
					filter = new ExcludeFilter(exclude);
				}
				else
				{
					filter = EmptyFilter.Instance;
				}

				_filterLookup.Assign<TFilterSelector>(filter);
			}

			return filter;
		}
	}
}
