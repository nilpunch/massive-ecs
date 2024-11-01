using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class DynamicFilter
	{
		private DynamicFilter Parent { get; set; }
		private DynamicFilter Child { get; set; }

		public Registry Registry { get; set; }

		public FastListSparseSet Included { get; } = new FastListSparseSet();
		public FastListSparseSet Excluded { get; } = new FastListSparseSet();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DynamicFilter Include<T>()
		{
			Included.Add(Registry.Set<T>());
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DynamicFilter Exclude<T>()
		{
			Excluded.Add(Registry.Set<T>());
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			Included.Clear();
			Excluded.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			IdsSource minimal = Included.Count == 0 ? Registry.Entities : SetHelpers.GetMinimalSet(Included.Items, Included.Count);
			return new Enumerator(minimal, this);
		}

		private static DynamicFilter _sharedParent = new DynamicFilter();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DynamicFilter GetShared(Registry registry)
		{
			if (_sharedParent.Child == null)
			{
				_sharedParent.Child = new DynamicFilter();
				_sharedParent.Child.Parent = _sharedParent;
				_sharedParent = _sharedParent.Child;
			}

			_sharedParent.Registry = registry;
			_sharedParent.Clear();
			return _sharedParent;
		}

		public struct Enumerator : IDisposable
		{
			private readonly IdsSource _idsSource;
			private readonly DynamicFilter _filter;
			private readonly SparseSet[] _include;
			private readonly SparseSet[] _exclude;
			private readonly int _includeCount;
			private readonly int _excludeCount;
			private int _index;
			private int _current;

			public Enumerator(IdsSource idsSource, DynamicFilter filter)
			{
				_idsSource = idsSource;
				_filter = filter;
				_include = filter.Included.Items;
				_exclude = filter.Excluded.Items;
				_includeCount = filter.Included.Count;
				_excludeCount = filter.Excluded.Count;
				_index = _idsSource.Count;
				_current = Constants.InvalidId;
			}

			public int Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _current;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (--_index > _idsSource.Count)
				{
					_index = _idsSource.Count - 1;
				}

				while (_index >= 0 && !ContainsId(_current = _idsSource.Ids[_index]))
				{
					--_index;
				}

				return _index >= 0;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private bool ContainsId(int id)
			{
				return id >= 0 && (_include.Length == 0 || SetHelpers.AssignedInAll(id, _include, _includeCount)) && (_exclude.Length == 0 || SetHelpers.NotAssignedInAll(id, _exclude, _excludeCount));
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose()
			{
				if (_filter.Parent != null)
				{
					_sharedParent = _filter.Parent;
				}
			}
		}
	}
}
