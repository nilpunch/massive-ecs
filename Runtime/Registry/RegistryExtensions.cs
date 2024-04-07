using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistryExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Identifier Create<T>(this IRegistry registry, T data = default) where T : struct
		{
			var id = registry.Create();
			registry.Add(id, data);
			return id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Identifier GetIdentifier(this IRegistry registry, int id)
		{
			return registry.Entities.GetIdentifier(id);
		}
	}
}