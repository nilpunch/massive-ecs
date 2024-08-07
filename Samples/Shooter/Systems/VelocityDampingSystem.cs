namespace Massive.Samples.Shooter
{
	public static class VelocityDampingSystem
	{
		public static void Update(Registry registry, float deltaTime)
		{
			registry.View().ForEachExtra(deltaTime,
				static (ref Velocity velocity, ref VelocityDamper damper, float deltaTime) =>
				{
					damper.DampVelocity(ref velocity, deltaTime);
				});
		}
	}
}
