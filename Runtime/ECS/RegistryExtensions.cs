using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	public static class RegistryExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity GetEntity(this Registry registry, int entityId)
		{
			return new Entity(registry, entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity CreateEntity<T>(this Registry registry, T data) where T : unmanaged
		{
			return new Entity(registry, registry.Create(data));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity CreateEntity(this Registry registry)
		{
			return new Entity(registry, registry.Create());
		}

		public static Filter Filter(this Registry registry)
		{
			return new Filter(registry);
		}

		public static View View(this Registry registry)
		{
			return new View(registry);
		}

		public static View<T> View<T>(this Registry registry) where T : unmanaged
		{
			return new View<T>(registry);
		}

		public static View<T1, T2> View<T1, T2>(this Registry registry) where T1 : unmanaged where T2 : unmanaged
		{
			return new View<T1, T2>(registry);
		}

		public static FilterView View(this Registry registry, Filter filter)
		{
			return new FilterView(registry, filter);
		}

		public static FilterView<T> View<T>(this Registry registry, Filter filter) where T : unmanaged
		{
			return new FilterView<T>(registry, filter);
		}

		public static FilterView<T1, T2> View<T1, T2>(this Registry registry, Filter filter) where T1 : unmanaged where T2 : unmanaged
		{
			return new FilterView<T1, T2>(registry, filter);
		}
	}
}