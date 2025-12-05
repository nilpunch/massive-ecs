#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class DynamicFilter
	{
		private Filter _filter;

		public Sets Sets { get; }

		public DynamicFilter(World world)
		{
			Sets = world.Sets;
			_filter = Filter.Empty;
		}

		public static implicit operator Filter(DynamicFilter dynamicFilter)
		{
			return dynamicFilter._filter;
		}

		public DynamicFilter Include<T>()
		{
			var set = Sets.Get<T>();

			if (_filter.Included.Contains(set))
			{
				return this;
			}

			if (_filter.IncludeCount >= _filter.Included.Length)
			{
				_filter.Included = _filter.Included.ResizeToNextPowOf2(_filter.IncludeCount + 1);
			}

			_filter.Included[_filter.IncludeCount] = set;
			_filter.IncludeCount += 1;

			FilterException.ThrowIfHasConflicts(_filter.Included, _filter.Excluded, FilterType.Included, FilterType.Excluded);

			return this;
		}

		public DynamicFilter Exclude<T>()
		{
			var set = Sets.Get<T>();

			if (_filter.Excluded.Contains(set))
			{
				return this;
			}

			if (_filter.ExcludeCount >= _filter.Excluded.Length)
			{
				_filter.Excluded = _filter.Excluded.ResizeToNextPowOf2(_filter.ExcludeCount + 1);
			}

			_filter.Excluded[_filter.ExcludeCount] = set;
			_filter.ExcludeCount += 1;

			FilterException.ThrowIfHasConflicts(_filter.Included, _filter.Excluded, FilterType.Included, FilterType.Excluded);

			return this;
		}
	}
}
