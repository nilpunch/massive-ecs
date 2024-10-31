namespace Massive.Samples.Shooter
{
	public static class VelocityDampingSystem
	{
		public static void Update(Registry registry, float deltaTime)
		{
			foreach (var id in registry.View().Filter<Include<Velocity, VelocityDamper>>())
			{
				ref var velocity = ref registry.Get<Velocity>(id);
				ref var damper = ref registry.Get<VelocityDamper>(id);
				
				damper.DampVelocity(ref velocity, deltaTime);
			}
		}
	}
}
