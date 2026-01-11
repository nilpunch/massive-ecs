#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldSetExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitSet BitSet<T>(this World world)
		{
			var info = TypeId<SetKind, T>.Info;
			var sets = world.Sets;

			sets.EnsureLookupByTypeAt(info.Index);
			var candidate = sets.LookupByTypeId[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			return sets.Get<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DataSet<T> DataSet<T>(this World world)
		{
			var info = TypeId<SetKind, T>.Info;
			var sets = world.Sets;

			sets.EnsureLookupByTypeAt(info.Index);
			var candidate = sets.LookupByTypeId[info.Index];

			if (candidate != null)
			{
				NoDataException.ThrowIfHasNoData(candidate, info.Type, DataAccessContext.WorldDataSet);
				return (DataSet<T>)candidate;
			}

			var set = sets.Get<T>();
			NoDataException.ThrowIfHasNoData(set, info.Type, DataAccessContext.WorldDataSet);

			return (DataSet<T>)set;
		}

		public static BitSet[] SelectSets<T>(this World world)
		{
			var info = TypeId<SelectorKind, Selector<T>>.Info;
			var sets = world.Sets;

			sets.EnsureSelectionLookupAt(info.Index);
			var candidate = sets.SelectionLookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			return sets.SelectSets<Selector<T>>();
		}

		public static BitSet[] SelectSets<T1, T2>(this World world)
		{
			var info = TypeId<SelectorKind, Selector<T1, T2>>.Info;
			var sets = world.Sets;

			sets.EnsureSelectionLookupAt(info.Index);
			var candidate = sets.SelectionLookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			return sets.SelectSets<Selector<T1, T2>>();
		}

		public static BitSet[] SelectSets<T1, T2, T3>(this World world)
		{
			var info = TypeId<SelectorKind, Selector<T1, T2, T3>>.Info;
			var sets = world.Sets;

			sets.EnsureSelectionLookupAt(info.Index);
			var candidate = sets.SelectionLookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			return sets.SelectSets<Selector<T1, T2, T3>>();
		}

		public static BitSet[] SelectSets<T1, T2, T3, T4>(this World world)
		{
			var info = TypeId<SelectorKind, Selector<T1, T2, T3, T4>>.Info;
			var sets = world.Sets;

			sets.EnsureSelectionLookupAt(info.Index);
			var candidate = sets.SelectionLookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			return sets.SelectSets<Selector<T1, T2, T3, T4>>();
		}

		public static BitSet[] SelectSets<T1, T2, T3, T4, T5>(this World world)
		{
			var info = TypeId<SelectorKind, Selector<T1, T2, T3, T4, T5>>.Info;
			var sets = world.Sets;

			sets.EnsureSelectionLookupAt(info.Index);
			var candidate = sets.SelectionLookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			return sets.SelectSets<Selector<T1, T2, T3, T4, T5>>();
		}

		public static BitSet[] SelectSets<T1, T2, T3, T4, T5, T6>(this World world)
		{
			var info = TypeId<SelectorKind, Selector<T1, T2, T3, T4, T5, T6>>.Info;
			var sets = world.Sets;

			sets.EnsureSelectionLookupAt(info.Index);
			var candidate = sets.SelectionLookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			return sets.SelectSets<Selector<T1, T2, T3, T4, T5, T6>>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitSet[] SelectSets<T1, T2, T3, T4, T5, T6, TSelector>(this World world)
			where TSelector : ISetSelector, new()
		{
			var info = TypeId<SelectorKind, Selector<T1, T2, T3, TSelector>>.Info;
			var sets = world.Sets;

			sets.EnsureSelectionLookupAt(info.Index);
			var candidate = sets.SelectionLookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			return sets.SelectSets<Selector<T1, T2, T3, T4, T5, T6, TSelector>>();
		}
	}
}
