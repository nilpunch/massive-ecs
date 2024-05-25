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
		private readonly List<int> _allSetsIndicesSorted = new List<int>();
		private readonly List<ISet> _allSets = new List<ISet>();
		private readonly ISetFactory _setFactory;
		private ISet[] _setsLookup = new ISet[16];

		public IndexedSetCollection(ISetFactory setFactory)
		{
			_setFactory = setFactory;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet Get<T>()
		{
			var typeIndex = TypeLookup<T>.Index;

			// Resize lookup to fit
			if (typeIndex >= _setsLookup.Length)
			{
				Array.Resize(ref _setsLookup, MathHelpers.GetNextPowerOf2(typeIndex + 1));
			}

			ISet set = _setsLookup[typeIndex];

			if (set == null)
			{
				set = _setFactory.CreateAppropriateSet<T>();
				_setsLookup[typeIndex] = set;

				// Maintain sets sorted
				int insertionIndex = ~_allSetsIndicesSorted.BinarySearch(typeIndex);
				_allSetsIndicesSorted.Insert(insertionIndex, typeIndex);
				_allSets.Insert(insertionIndex, set);
			}

			return set;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public List<ISet>.Enumerator GetEnumerator()
		{
			return _allSets.GetEnumerator();
		}

		private static class TypeLookup<T>
		{
			// ReSharper disable once StaticMemberInGenericType
			public static readonly int Index;

			static TypeLookup()
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
