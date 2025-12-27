namespace Massive
{
	public static class SystemsInjectionExtensions
	{
		/// <summary>
		/// Calls <see cref="ISystemInject{TArg}.Inject"/> on all builded systems.
		/// </summary>
		public static Systems Inject<TArg>(this Systems systems, TArg arg)
		{
			return systems.Run<ISystemInject<TArg>, TArg>(arg);
		}

		/// <summary>
		/// Builds systems and calls <see cref="ISystemInject{TArg}.Inject"/> with <see cref="World"/> argument on all builded systems.
		/// </summary>
		public static Systems Build(this Systems systems, World world)
		{
			return systems.Build().Inject(world);
		}
	}
}
