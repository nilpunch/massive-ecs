using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class SetRegistry
	{
		private readonly GenericLookup<SparseSet> _setLookup = new GenericLookup<SparseSet>();
		private readonly ISetFactory _setFactory;

		public SetRegistry(ISetFactory setFactory)
		{
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
			var set = _setLookup.GetOrDefault<TKey>();

			if (set == null)
			{
				set = (SparseSet)_setFactory.CreateAppropriateSet<TKey>();
				_setLookup.Assign<TKey>(set);
			}

			return set;
		}
	}
}
