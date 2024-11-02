using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class SetRegistry
	{
		private readonly GenericLookupSparseSet _setLookup;
		private readonly ISetFactory _setFactory;

		public SetRegistry(ISetFactory setFactory)
		{
			_setLookup = new GenericLookupSparseSet();
			_setFactory = setFactory;
		}

		public ReadOnlySpan<SparseSet> All
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _setLookup.All;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet Get<TKey>()
		{
			var set = _setLookup.Find<TKey>();

			if (set is null)
			{
				set = _setFactory.CreateAppropriateSet<TKey>();
				_setLookup.Assign<TKey>(set);
			}

			return set;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet Get(Type keyType)
		{
			var set = _setLookup.Find(keyType);

			if (set is null)
			{
				set = _setFactory.CreateAppropriateSet(keyType);
				_setLookup.Assign(keyType, set);
			}

			return set;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Assign(Type type, SparseSet sparseSet)
		{
			_setLookup.Assign(type, sparseSet);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Type GetKey(SparseSet set)
		{
			return _setLookup.GetKey(set);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(SparseSet set)
		{
			return _setLookup.IndexOf(set);
		}
	}
}
