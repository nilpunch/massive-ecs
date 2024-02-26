using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	public static class RegistryExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity GetEntity(this IRegistry registry, int entityId)
		{
			return new Entity(registry, entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity CreateEntity<T>(this IRegistry registry, T data) where T : unmanaged
		{
			return new Entity(registry, registry.Create(data));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Create<T>(this IRegistry registry, T data) where T : unmanaged
		{
			int id = registry.Create();
			registry.Add(id, data);
			return id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity CreateEntity(this IRegistry registry)
		{
			return new Entity(registry, registry.Create());
		}

		public static Filter Filter(this IRegistry registry)
		{
			return new Filter(registry);
		}

		public static View View(this IRegistry registry)
		{
			return new View(registry);
		}

		public static View<T> View<T>(this IRegistry registry) where T : unmanaged
		{
			return new View<T>(registry);
		}

		public static View<T1, T2> View<T1, T2>(this IRegistry registry) where T1 : unmanaged where T2 : unmanaged
		{
			return new View<T1, T2>(registry);
		}

		public static FilterView View(this IRegistry registry, Filter filter)
		{
			return new FilterView(registry, filter);
		}

		public static FilterView<T> View<T>(this IRegistry registry, Filter filter) where T : unmanaged
		{
			return new FilterView<T>(registry, filter);
		}

		public static FilterView<T1, T2> View<T1, T2>(this IRegistry registry, Filter filter) where T1 : unmanaged where T2 : unmanaged
		{
			return new FilterView<T1, T2>(registry, filter);
		}
	}
}