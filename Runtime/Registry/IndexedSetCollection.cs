using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class IndexedSetCollection
	{
		private readonly List<ISet> _allSets = new List<ISet>();
		private readonly ISetFactory _setFactory;
		private ISet[] _setsLookup = new ISet[16];

		public IReadOnlyList<ISet> AllSets => _allSets;

		public IndexedSetCollection(ISetFactory setFactory)
		{
			_setFactory = setFactory;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet Get<T>()
		{
			var typeIndex = CachedType<T>.Index;

			ISet set = typeIndex < _setsLookup.Length ? _setsLookup[typeIndex] : null;

			if (set == null)
			{
				set = _setFactory.CreateAppropriateSet<T>();
				_allSets.Add(set);

				if (typeIndex >= _setsLookup.Length)
				{
					Array.Resize(ref _setsLookup, MathHelpers.GetNextPowerOf2(typeIndex + 1));
				}

				_setsLookup[typeIndex] = set;
			}

			return set;
		}

		private static class CachedType<T>
		{
			// ReSharper disable once StaticMemberInGenericType
			public static readonly int Index;

			static CachedType()
			{
				Index = IndexCounter.NextIndex;
				IndexCounter.NextIndex += 1;
			}
		}

		private static class IndexCounter
		{
			public static int NextIndex;
		}
	}
}
