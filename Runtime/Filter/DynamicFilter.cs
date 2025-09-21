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

		public DynamicFilter AddToAll<T>()
		{
			var set = Sets.Get<T>();

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

			FilterException.ThrowIfHasConflicts(_filter.All, _filter.None, FilterType.All, FilterType.None);
			FilterException.ThrowIfHasConflicts(_filter.All, _filter.Any, FilterType.All, FilterType.Any);

			return this;
		}

		public DynamicFilter AddToNone<T>()
		{
			var set = Sets.Get<T>();

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

			FilterException.ThrowIfHasConflicts(_filter.All, _filter.None, FilterType.All, FilterType.None);
			FilterException.ThrowIfHasConflicts(_filter.None, _filter.Any, FilterType.None, FilterType.Any);

			return this;
		}
	}
}
