using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	public static class RegistryExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Create<T>(this IRegistry registry, T data = default) where T : struct
		{
			int id = registry.Create();
			registry.Add(id, data);
			return id;
		}
	}
}