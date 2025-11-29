using Massive.Samples.UpdateLoop;

namespace Massive.Samples.StatefulRollbacks
{
	public class Sample
	{
		private readonly Systems _systems;

		public Sample()
		{
			_systems = new Systems()
				.New<PhysicsSystem>()
				.Build(new World());

			_systems.Run<IInitinalize>();
		}

		public void UpdateLoop(int tick)
		{
			if (tick == 0)
			{
				_systems.Run<IFirstTick>();
			}

			var deltaTime = 1f / 60;

			_systems.Run<IUpdate, float>(deltaTime);
		}
	}
}