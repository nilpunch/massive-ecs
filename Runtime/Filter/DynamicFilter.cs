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

			ConflictingFilterException.ThrowIfConflictWithExcluded(this, set);

			if (_filter.All.Contains(set))
			{
				return this;
			}

			if (_filter.AllCount >= _filter.All.Length)
			{
				_filter.All = _filter.All.ResizeToNextPowOf2(_filter.AllCount + 1);
			}

			_filter.All[_filter.AllCount] = set;
			_filter.AllCount += 1;

			return this;
		}

		public DynamicFilter Exclude<T>()
		{
			var set = Sets.Get<T>();

			ConflictingFilterException.ThrowIfConflictWithIncluded(this, set);

			if (_filter.None.Contains(set))
			{
				return this;
			}

			if (_filter.NoneCount >= _filter.None.Length)
			{
				_filter.None = _filter.None.ResizeToNextPowOf2(_filter.NoneCount + 1);
			}

			_filter.None[_filter.NoneCount] = set;
			_filter.NoneCount += 1;

			return this;
		}
	}
}
