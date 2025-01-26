#if !MASSIVE_RELEASE
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class RegistrySetExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DataSet<T> DataSet<T>(this Registry registry)
		{
			Assert.TypeHasData<T>(registry, SuggestionMessage.UseSetMethodWithEmptyTypes);

			return registry.Set<T>() as DataSet<T>;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SparseSet Set<T>(this Registry registry)
		{
			return registry.SetRegistry.Get<T>();
		}
	}
}
