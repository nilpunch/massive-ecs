namespace Massive.Samples.Shooter
{
	public class VelocityDampingSystem : WorldSystem, IUpdate
	{
		public void Update(float deltaTime)
		{
			World.ForEach(deltaTime,
				static (ref Velocity velocity, ref VelocityDamper damper, float deltaTime) =>
				{
					damper.DampVelocity(ref velocity, deltaTime);
				});
		}
	}
}
