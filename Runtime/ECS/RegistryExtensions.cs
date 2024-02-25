namespace Massive.ECS
{
	public static class RegistryExtensions
	{
		public static View View(this Registry registry)
		{
			return new View(registry.Entities);
		}

		public static View<T> View<T>(this Registry registry) where T : unmanaged
		{
			return new View<T>(registry.Component<T>());
		}

		public static View<T1, T2> View<T1, T2>(this Registry registry) where T1 : unmanaged where T2 : unmanaged
		{
			return new View<T1, T2>(registry.Component<T1>(), registry.Component<T2>());
		}

		public static Filter Filter(this Registry registry)
		{
			return new Filter(registry);
		}

		public static FilterView View(this Registry registry, Filter filter)
		{
			return new FilterView(registry.Entities, filter);
		}

		public static FilterView<T> View<T>(this Registry registry, Filter filter) where T : unmanaged
		{
			return new FilterView<T>(registry.Component<T>(), filter);
		}

		public static FilterView<T1, T2> View<T1, T2>(this Registry registry, Filter filter) where T1 : unmanaged where T2 : unmanaged
		{
			return new FilterView<T1, T2>(registry.Component<T1>(), registry.Component<T2>(), filter);
		}
	}
}