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
		public static BitSet BitSet<T>(this World world)
		{
			var info = ComponentId<T>.Info;
			var sets = world.BitSets;

			sets.EnsureLookupAt(info.Index);
			var candidate = sets.Lookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			return sets.Get<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DataSet<T> DataSet<T>(this World world)
		{
			var info = ComponentId<T>.Info;
			var sets = world.BitSets;

			sets.EnsureLookupAt(info.Index);
			var candidate = sets.Lookup[info.Index];

			if (candidate != null)
			{
				NoDataException.ThrowIfHasNoData(candidate, info.Type, DataAccessContext.WorldDataSet);
				return (DataSet<T>)candidate;
			}

			var set = sets.Get<T>();
			NoDataException.ThrowIfHasNoData(set, info.Type, DataAccessContext.WorldDataSet);

			return (DataSet<T>)set;
		}
	}
}
