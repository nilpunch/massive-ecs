using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

// ReSharper disable StaticMemberInGenericType
namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class GenericLookup<TAbstract>
	{
		private readonly FastList<string> _itemIds = new FastList<string>();
		private readonly FastList<TAbstract> _items = new FastList<TAbstract>();
		private readonly TAbstract[] _lookup;

		public GenericLookup(int maxTypesAmount = Constants.DefaultMaxTypesAmount)
		{
			_lookup = new TAbstract[maxTypesAmount];
			Array.Fill(_lookup, default);
		}

		public ReadOnlySpan<TAbstract> All
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _items.ReadOnlySpan;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TAbstract GetOrDefault<TKey>()
		{
			return _lookup[TypeLookup<TKey>.Index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TAbstract GetOrDefault(int index)
		{
			return _lookup[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndex<TKey>()
		{
			return TypeLookup<TKey>.Index;
		}

		public void Assign<TKey>(TAbstract item)
		{
			var typeIndex = TypeLookup<TKey>.Index;

			_lookup[typeIndex] = item;

			// Maintain items sorted
			var itemId = TypeLookup<TKey>.FullName;
			var itemIndex = _itemIds.BinarySearch(itemId);
			if (itemIndex >= 0)
			{
				_items[itemIndex] = item;
			}
			else
			{
				var insertionIndex = ~itemIndex;
				_itemIds.Insert(insertionIndex, itemId);
				_items.Insert(insertionIndex, item);
			}
		}

		private static class TypeLookup<TKey>
		{
			public static int Index { get; }
			public static string FullName { get; }

			static TypeLookup()
			{
				Index = IndexCounter.NextIndex++;
				FullName = typeof(TKey).GetFullBeautifulName();
			}
		}

		private static class IndexCounter
		{
			public static int NextIndex { get; set; }
		}
	}
}
