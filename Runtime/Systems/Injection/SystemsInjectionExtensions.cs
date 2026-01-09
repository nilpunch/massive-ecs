namespace Massive
{
	public static class SystemsInjectionExtensions
	{
		/// <summary>
		/// Calls <see cref="IInject{TArg}.Inject"/> on all builded systems.
		/// </summary>
		public static Systems Inject<TArg>(this Systems systems, TArg arg)
		{
			foreach (var system in systems.GetAllSystems())
			{
				if (system is IInject<TArg> injectMethod)
				{
					injectMethod.Inject(arg);
				}
			}

			return systems;
		}

		/// <summary>
		/// Builds systems and calls <see cref="IInject{TArg}.Inject"/> with <see cref="World"/> argument on all builded systems.
		/// </summary>
		public static Systems Build(this Systems systems, World world)
		{
			return systems.Build().Inject(world);
		}
	}
}
