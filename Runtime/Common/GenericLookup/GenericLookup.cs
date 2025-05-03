using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class GenericLookup<TAbstract>
	{
		private readonly FastList<string> _itemIds = new FastList<string>();
		private readonly FastList<TAbstract> _items = new FastList<TAbstract>();
		private TAbstract[] _lookup = Array.Empty<TAbstract>();
		private Type[] _keyLookup = Array.Empty<Type>();

		public FastList<TAbstract> All
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _items;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TAbstract Find<TKey>()
		{
			var typeIndex = TypeId<TKey>.Info.Index;

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
		public TAbstract Find(string id)
		{
			var itemIndex = _itemIds.BinarySearch(id);
			if (itemIndex >= 0)
			{
				return _items[itemIndex];
			}
			else
			{
				return default;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf<TKey>()
		{
			return TypeId<TKey>.Info.Index;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(TAbstract item)
		{
			return Array.IndexOf(_lookup, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Type GetKey(TAbstract item)
		{
			return _keyLookup[Array.IndexOf(_lookup, item)];
		}

		public void Assign(string itemId, TAbstract item)
		{
			// Maintain items sorted.
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

		public void Assign<TKey>(TAbstract item)
		{
			var typeInfo = TypeId<TKey>.Info;
			var typeIndex = typeInfo.Index;

			// Resize lookup to fit.
			if (typeIndex >= _lookup.Length)
			{
				Array.Resize(ref _lookup, MathUtils.NextPowerOf2(typeIndex + 1));
				Array.Resize(ref _keyLookup, MathUtils.NextPowerOf2(typeIndex + 1));
			}

			_lookup[typeIndex] = item;
			_keyLookup[typeIndex] = typeof(TKey);

			Assign(typeInfo.FullName, item);
		}

		public TAbstract Find(Type key)
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
			return TypeId.GetInfo(key).Index;
		}

		public void Assign(Type keyType, TAbstract item)
		{
			var typeInfo = TypeId.GetInfo(keyType);
			var typeIndex = typeInfo.Index;

			// Resize lookup to fit.
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
