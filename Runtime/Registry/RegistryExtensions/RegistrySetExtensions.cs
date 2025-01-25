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
			Debug.AssertNotEmptyType<T>(registry, SuggestionMessage.UseSetMethod);

			return registry.Set<T>() as DataSet<T>;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SparseSet Set<T>(this Registry registry)
		{
			return registry.SetRegistry.Get<T>();
		}
	}
}
