using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class SetRegistry
	{
		private readonly GenericLookup<SparseSet> _setLookup = new GenericLookup<SparseSet>();
		private readonly ISetFactory _setFactory;

		public SetRegistry(ISetFactory setFactory)
		{
			_setFactory = setFactory;
		}

		public event Action<SparseSet, int> SetCreated;

		public ReadOnlySpan<SparseSet> All
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _setLookup.All;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet Get<TKey>()
		{
			var set = _setLookup.GetOrDefault<TKey>();

			if (set == null)
			{
				set = _setFactory.CreateAppropriateSet<TKey>();
				_setLookup.Assign<TKey>(set);
				SetCreated?.Invoke(set, _setLookup.GetIndex<TKey>());
			}

			return set;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet FindSetById(int id)
		{
			return _setLookup.GetOrDefault(id);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetId<TKey>()
		{
			return _setLookup.GetIndex<TKey>();
		}
	}
}
