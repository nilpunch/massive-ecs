namespace Massive.Recursion
{
	public static class SystemsRecusrionExtensions
	{
		public static Systems RunRecursion<TSystemMethod>(this Systems systems)
			where TSystemMethod : IRecursiveSystemMethod<TSystemMethod>
		{
			var methods = systems.GetSystemsOfType<TSystemMethod>();
			bool running;
			do
			{
				running = false;
				foreach (var method in methods)
				{
					running |= method.Run() == RunResult.Active;
				}
			} while (running);

			return systems;
		}

		public static Systems RunRecursion<TSystemMethod, TArgs>(this Systems systems, TArgs args)
			where TSystemMethod : IRecursiveSystemMethod<TSystemMethod, TArgs>
		{
			var methods = systems.GetSystemsOfType<TSystemMethod>();
			bool running;
			do
			{
				running = false;
				foreach (var method in methods)
				{
					running |= method.Run(args) == RunResult.Active;
				}
			} while (running);

			return systems;
		}
	}
}
