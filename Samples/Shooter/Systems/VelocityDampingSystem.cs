namespace Massive.Samples.Shooter
{
	public static class VelocityDampingSystem
	{
		public static void Update(IRegistry registry, float deltaTime)
		{
			registry.View().ForEachExtra(deltaTime,
				(ref Velocity velocity, ref VelocityDamper damper, float deltaTime) =>
			{
				damper.DampVelocity(ref velocity, deltaTime);
			});
		}
	}
}
