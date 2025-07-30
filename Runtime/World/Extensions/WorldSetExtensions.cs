#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldSetExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SparseSet SparseSet<T>(this World world)
		{
			var info = TypeId<T>.Info;
			var sets = world.Sets;

			sets.EnsureLookupAt(info.Index);
			var candidate = sets.Lookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			var collapsedInfo = TypeId.GetInfo(NegativeUtility.Collapse(info.Type));
			if (collapsedInfo.Type != info.Type)
			{
				var collapsedSet = sets.GetReflected(collapsedInfo.Type);
				sets.Lookup[info.Index] = collapsedSet;
				return collapsedSet;
			}

			var (set, cloner) = sets.SetFactory.CreateAppropriateSet<T>();

			sets.Insert(info.FullName, set, cloner);
			sets.Lookup[info.Index] = set;

			sets.TryCreatePairedSet(set, info);

			return set;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DataSet<T> DataSet<T>(this World world)
		{
			var info = TypeId<T>.Info;
			var sets = world.Sets;

			sets.EnsureLookupAt(info.Index);
			var candidate = sets.Lookup[info.Index];

			if (candidate != null)
			{
				NoDataException.ThrowIfHasNoData(candidate, typeof(T), DataAccessContext.WorldDataSet);
				return (DataSet<T>)candidate;
			}

			var collapsedInfo = TypeId.GetInfo(NegativeUtility.Collapse(info.Type));
			if (collapsedInfo.Type != info.Type)
			{
				var collapsedSet = sets.GetReflected(collapsedInfo.Type);
				NoDataException.ThrowIfHasNoData(collapsedSet, collapsedInfo.Type, DataAccessContext.WorldDataSet);

				sets.Lookup[info.Index] = collapsedSet;
				return (DataSet<T>)collapsedSet;
			}

			var (set, cloner) = sets.SetFactory.CreateAppropriateSet<T>();
			NoDataException.ThrowIfHasNoData(set, typeof(T), DataAccessContext.WorldDataSet);

			sets.Insert(info.FullName, set, cloner);
			sets.Lookup[info.Index] = set;

			sets.TryCreatePairedSet(set, info);

			return (DataSet<T>)set;
		}
	}
}
