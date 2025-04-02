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

			var createdSet = setRegistry.SetFactory.CreateAppropriateSet<T>();

			setRegistry.Insert(info.FullName, createdSet);
			setRegistry.Lookup[info.Index] = createdSet;

			return createdSet;
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
				Assert.TypeHasData(candidate, typeof(T), SuggestionMessage.UseSetMethodWithEmptyTypes);
				return (DataSet<T>)candidate;
			}

			var createdSet = setRegistry.SetFactory.CreateAppropriateSet<T>();
			Assert.TypeHasData(createdSet, typeof(T), SuggestionMessage.UseSetMethodWithEmptyTypes);

			setRegistry.Insert(info.FullName, createdSet);
			setRegistry.Lookup[info.Index] = createdSet;

			return (DataSet<T>)createdSet;
		}
	}
}
