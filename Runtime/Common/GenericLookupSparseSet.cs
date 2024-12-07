using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class GenericLookupSparseSet
	{
		private readonly FastList<string> _itemIds = new FastList<string>();
		private readonly FastListSparseSet _items = new FastListSparseSet();
		private SparseSet[] _lookup = Array.Empty<SparseSet>();
		private Type[] _keyLookup = Array.Empty<Type>();

		public FastListSparseSet All
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _items;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet Find<TKey>()
		{
			var typeIndex = TypeIdentifier<TKey>.Info.Index;

			if (typeIndex >= _lookup.Length)
			{
				return default;
			}

			return _lookup[typeIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet Find(int index)
		{
			if (index >= _lookup.Length)
			{
				return default;
			}

			return _lookup[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet Find(string id)
		{
			var itemIndex = _itemIds.BinarySearch(id);
			if (itemIndex >= 0)
			{
				return _items[itemIndex];
			}
			else
			{
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf<TKey>()
		{
			return TypeIdentifier<TKey>.Info.Index;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(SparseSet item)
		{
			return Array.IndexOf(_lookup, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Type GetKey(SparseSet item)
		{
			return _keyLookup[Array.IndexOf(_lookup, item)];
		}

		public void Assign(string itemId, SparseSet item)
		{
			// Maintain items sorted
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

		public void Assign<TKey>(SparseSet item)
		{
			var typeInfo = TypeIdentifier<TKey>.Info;
			var typeIndex = typeInfo.Index;

			// Resize lookup to fit
			if (typeIndex >= _lookup.Length)
			{
				Array.Resize(ref _lookup, MathUtils.NextPowerOf2(typeIndex + 1));
				Array.Resize(ref _keyLookup, MathUtils.NextPowerOf2(typeIndex + 1));
			}

			_lookup[typeIndex] = item;
			_keyLookup[typeIndex] = typeof(TKey);

			// Maintain items sorted
			var itemId = typeInfo.FullName;
			Assign(itemId, item);
		}

		public SparseSet Find(Type key)
		{
			var typeIndex = IndexOf(key);

			if (typeIndex >= _lookup.Length)
			{
				return default;
			}

			return _lookup[typeIndex];
		}

		public int IndexOf(Type key)
		{
			return TypeIdentifier.GetInfo(key).Index;
		}

		public void Assign(Type keyType, SparseSet item)
		{
			var typeInfo = TypeIdentifier.GetInfo(keyType);
			var typeIndex = typeInfo.Index;

			// Resize lookup to fit
			if (typeIndex >= _lookup.Length)
			{
				Array.Resize(ref _lookup, MathUtils.NextPowerOf2(typeIndex + 1));
				Array.Resize(ref _keyLookup, MathUtils.NextPowerOf2(typeIndex + 1));
			}

			_lookup[typeIndex] = item;
			_keyLookup[typeIndex] = keyType;

			Assign(typeInfo.FullName, item);
		}
	}
}
