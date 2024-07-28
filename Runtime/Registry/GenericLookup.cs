using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

// ReSharper disable StaticMemberInGenericType
namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class GenericLookup<TAbstract>
	{
		private readonly List<string> _itemIds = new List<string>();
		private readonly List<TAbstract> _items = new List<TAbstract>();
		private TAbstract[] _lookup = new TAbstract[16];

		public IReadOnlyList<TAbstract> All => _items;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TAbstract GetOrDefault<TKey>()
		{
			var typeIndex = TypeLookup<TKey>.Index;

			if (typeIndex >= _lookup.Length)
			{
				return default;
			}

			return _lookup[typeIndex];
		}

		public void Assign<TKey>(TAbstract item)
		{
			var typeIndex = TypeLookup<TKey>.Index;

			// Resize lookup to fit
			if (typeIndex >= _lookup.Length)
			{
				Array.Resize(ref _lookup, MathHelpers.GetNextPowerOf2(typeIndex + 1));
			}

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
