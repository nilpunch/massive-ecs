namespace Massive.Samples.Shooter
{
	public static class VelocityDampingSystem
	{
		public static void Update(World world, float deltaTime)
		{
			world.View().ForEach(deltaTime,
				static (ref Velocity velocity, ref VelocityDamper damper, float deltaTime) =>
				{
					damper.DampVelocity(ref velocity, deltaTime);
				});
		}
	}
}
