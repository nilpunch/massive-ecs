using System;
using System.Reflection;
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
		private TAbstract[] _lookup = new TAbstract[Constants.DefaultCapacity];
		private Type[] _typeLookup = new Type[Constants.DefaultCapacity];

		public ReadOnlySpan<TAbstract> All
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _items.ReadOnlySpan;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TAbstract Find<TKey>()
		{
			var typeIndex = TypeLookup<TKey>.Index;

			if (typeIndex >= _lookup.Length)
			{
				return default;
			}

			return _lookup[typeIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TAbstract Find(int index)
		{
			if (index >= _lookup.Length)
			{
				return default;
			}

			return _lookup[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf<TKey>()
		{
			return TypeLookup<TKey>.Index;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(TAbstract item)
		{
			return Array.IndexOf(_lookup, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Type TypeOf(TAbstract item)
		{
			return _typeLookup[Array.IndexOf(_lookup, item)];
		}

		public void Assign<TKey>(TAbstract item)
		{
			var typeIndex = TypeLookup<TKey>.Index;

			// Resize lookup to fit
			if (typeIndex >= _lookup.Length)
			{
				Array.Resize(ref _lookup, MathHelpers.NextPowerOf2(typeIndex + 1));
				Array.Resize(ref _typeLookup, MathHelpers.NextPowerOf2(typeIndex + 1));
			}

			_lookup[typeIndex] = item;
			_typeLookup[typeIndex] = typeof(TKey);

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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TAbstract Find(Type keyType)
		{
			var typeIndex = IndexOf(keyType);

			if (typeIndex >= _lookup.Length)
			{
				return default;
			}

			return _lookup[typeIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(Type keyType)
		{
			var property = MakeTypeLookupType(keyType).GetProperty("Index", BindingFlags.Public | BindingFlags.Static);
			return (int)property.GetValue(null);
		}

		public void Assign(Type keyType, TAbstract item)
		{
			var typeLookup = MakeTypeLookupType(keyType);
			
			var typeIndex = (int)typeLookup.GetProperty("Index", BindingFlags.Public | BindingFlags.Static).GetValue(null);
			var typeFullName = (string)typeLookup.GetProperty("FullName", BindingFlags.Public | BindingFlags.Static).GetValue(null);

			// Resize lookup to fit
			if (typeIndex >= _lookup.Length)
			{
				Array.Resize(ref _lookup, MathHelpers.NextPowerOf2(typeIndex + 1));
				Array.Resize(ref _typeLookup, MathHelpers.NextPowerOf2(typeIndex + 1));
			}

			_lookup[typeIndex] = item;
			_typeLookup[typeIndex] = keyType;

			// Maintain items sorted
			var itemId = typeFullName;
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

		private Type MakeTypeLookupType(Type keyType)
		{
			return typeof(TypeLookup<>).MakeGenericType(typeof(TAbstract), keyType);
		}

		private static class TypeLookup<TKey>
		{
			public static int Index { get; }
			public static string FullName { get; }

			static TypeLookup()
			{
				Index = IndexCounter.NextIndex++;
				FullName = typeof(TKey).GetFullGenericName();
			}
		}

		private static class IndexCounter
		{
			public static int NextIndex { get; set; }
		}
	}
}
