using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class SetRegistry
	{
		private readonly GenericLookup<SparseSet> _setLookup;
		private readonly ISetFactory _setFactory;

		public SetRegistry(ISetFactory setFactory)
		{
			_setLookup = new GenericLookup<SparseSet>();
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
			var set = _setLookup.Find<TKey>();

			if (set is null)
			{
				set = _setFactory.CreateAppropriateSet<TKey>();
				_setLookup.Assign<TKey>(set);
				SetCreated?.Invoke(set, _setLookup.IndexOf<TKey>());
			}

			return set;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet Find(int id)
		{
			return _setLookup.Find(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(SparseSet set)
		{
			return _setLookup.IndexOf(set);
		}
	}
}
