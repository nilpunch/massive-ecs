#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class WorldSetExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SparseSet SparseSet<T>(this World world)
		{
			var info = TypeId<T>.Info;
			var setRegistry = world.SetRegistry;

			setRegistry.EnsureLookupAt(info.Index);
			var candidate = setRegistry.Lookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			var (set, cloner) = setRegistry.SetFactory.CreateAppropriateSet<T>();

			setRegistry.Insert(info.FullName, set, cloner);
			setRegistry.Lookup[info.Index] = set;

			return set;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DataSet<T> DataSet<T>(this World world)
		{
			var info = TypeId<T>.Info;
			var setRegistry = world.SetRegistry;

			setRegistry.EnsureLookupAt(info.Index);
			var candidate = setRegistry.Lookup[info.Index];

			if (candidate != null)
			{
				Assert.TypeHasData(candidate, typeof(T), SuggestionMessage.UseSparseSetMethodWithEmptyTypes);
				return (DataSet<T>)candidate;
			}

			var (set, cloner) = setRegistry.SetFactory.CreateAppropriateSet<T>();
			Assert.TypeHasData(set, typeof(T), SuggestionMessage.UseSparseSetMethodWithEmptyTypes);

			setRegistry.Insert(info.FullName, set, cloner);
			setRegistry.Lookup[info.Index] = set;

			return (DataSet<T>)set;
		}
	}
}
