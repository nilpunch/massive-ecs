namespace Massive
{
	public class SystemBase : ISystem
	{
		public int Id { get; private set; }
		public World World { get; private set; }

		void ISystem.Initialize(int id, Allocator _, World world)
		{
			Id = id;
			World = world;
		}
	}
}
