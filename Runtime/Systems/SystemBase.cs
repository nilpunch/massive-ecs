namespace Massive
{
	public class SystemBase : ISystem
	{
		public World World { get; private set; }
		public int Id { get; private set; }

		public void Initialize(World world, int id, Allocator _)
		{
			World = world;
			Id = id;
		}
	}
}
