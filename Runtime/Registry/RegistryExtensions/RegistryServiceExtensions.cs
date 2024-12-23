using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class RegistryServiceExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Service<T>(this Registry registry)
		{
			return registry.ServiceRegistry.Find<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AssignService<T>(this Registry registry, T instance)
		{
			registry.ServiceRegistry.Assign(instance);
		}
	}
}
