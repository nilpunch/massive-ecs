using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class TypeRegistry<TAbstract>
	{
		private readonly List<int> _allIndicesSorted = new List<int>();
		private readonly List<TAbstract> _all = new List<TAbstract>();
		private TAbstract[] _lookup = new TAbstract[16];

		public IReadOnlyList<TAbstract> All => _all;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected TAbstract GetOrNull<TKey>()
		{
			var typeIndex = TypeLookup<TKey>.Index;

			// Resize lookup to fit
			if (typeIndex >= _lookup.Length)
			{
				Array.Resize(ref _lookup, MathHelpers.GetNextPowerOf2(typeIndex + 1));
			}

			return _lookup[typeIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void Bind<TKey>(TAbstract item)
		{
			var typeIndex = TypeLookup<TKey>.Index;

			_lookup[typeIndex] = item;

			// Maintain items sorted
			int insertionIndex = ~_allIndicesSorted.BinarySearch(typeIndex);
			_allIndicesSorted.Insert(insertionIndex, typeIndex);
			_all.Insert(insertionIndex, item);
		}

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
