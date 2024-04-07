using System;
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
		public static void Destroy(this IRegistry registry, Identifier identifier)
		{
			if (!registry.Entities.IsAlive(identifier))
			{
				return;
			}

			registry.Destroy(identifier.Id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(this IRegistry registry, Identifier identifier)
		{
			return registry.Entities.IsAlive(identifier);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Add<T>(this IRegistry registry, Identifier identifier, T data = default) where T : struct
		{
			if (!registry.Entities.IsAlive(identifier))
			{
				return;
			}

			registry.Add(identifier.Id, data);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Remove<T>(this IRegistry registry, Identifier identifier) where T : struct
		{
			if (!registry.Entities.IsAlive(identifier))
			{
				return;
			}

			registry.Remove<T>(identifier.Id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this IRegistry registry, Identifier identifier) where T : struct
		{
			if (!registry.Entities.IsAlive(identifier))
			{
				return false;
			}

			return registry.Has<T>(identifier.Id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Get<T>(this IRegistry registry, Identifier identifier) where T : struct
		{
			if (!registry.Entities.IsAlive(identifier))
			{
				throw new Exception("Entity is not alive!");
			}

			return ref registry.Get<T>(identifier.Id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Identifier GetIdentifier(this IRegistry registry, int id)
		{
			return registry.Entities.GetIdentifier(id);
		}
	}
}