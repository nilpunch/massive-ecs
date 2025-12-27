namespace Massive
{
	public class SystemBase : ISystem, ISystemInject<World>
	{
		public int Id { get; private set; }
		public World World { get; private set; }

		void ISystem.Build(int id, Allocator _)
		{
			Id = id;
		}

		void ISystemInject<World>.Inject(World world)
		{
			World = world;
		}
	}
}
