using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class GenericLookup<TAbstract>
	{
		private readonly List<int> _allIndicesSorted = new List<int>();
		private readonly List<TAbstract> _allSorted = new List<TAbstract>();
		private TAbstract[] _lookup = new TAbstract[16];

		public IReadOnlyList<TAbstract> All => _allSorted;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TAbstract GetOrDefault<TKey>()
		{
			var typeIndex = TypeLookup<TKey>.Index;

			if (typeIndex >= _lookup.Length)
			{
				return default;
			}

			return _lookup[typeIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Assign<TKey>(TAbstract item)
		{
			var typeIndex = TypeLookup<TKey>.Index;

			// Resize lookup to fit
			if (typeIndex >= _lookup.Length)
			{
				Array.Resize(ref _lookup, MathHelpers.GetNextPowerOf2(typeIndex + 1));
			}

			_lookup[typeIndex] = item;

			// Maintain items sorted
			int itemIndex = _allIndicesSorted.BinarySearch(typeIndex);
			if (itemIndex >= 0)
			{
				_allSorted[itemIndex] = item;
			}
			else
			{
				int insertionIndex = ~itemIndex;
				_allIndicesSorted.Insert(insertionIndex, typeIndex);
				_allSorted.Insert(insertionIndex, item);
			}
		}

		// ReSharper disable once UnusedTypeParameter
		private static class TypeLookup<TKey>
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
			// ReSharper disable once StaticMemberInGenericType
			public static int NextIndex;
		}
	}
}
