using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class FilterRegistry : TypeRegistry<IFilter>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IFilter Get<TFilterSelector>(TFilterSelector selector)
			where TFilterSelector : struct, IFilterSelector
		{
			var filter = GetOrNull<TFilterSelector>();

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
				else
				{
					filter = new ExcludeFilter(exclude);
				}

				Bind<TFilterSelector>(filter);
			}

			return filter;
		}
	}
}
