using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class SetRegistry
	{
		private readonly GenericLookup<ISet> _setLookup = new GenericLookup<ISet>();
		private readonly ISetFactory _setFactory;

		public SetRegistry(ISetFactory setFactory)
		{
			_setFactory = setFactory;
		}

		public IReadOnlyList<ISet> All => _setLookup.All;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet Get<TKey>()
		{
			var set = _setLookup.GetOrDefault<TKey>();

			if (set == null)
			{
				set = _setFactory.CreateAppropriateSet<TKey>();
				_setLookup.Assign<TKey>(set);
			}

			return set;
		}
	}
}
