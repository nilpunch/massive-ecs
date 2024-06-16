namespace Massive.Samples.Shooter
{
	/// <summary>
	/// Move entities according to their velocities.
	/// </summary>
	public static class MovementSystem
	{
		public static void Update(IRegistry registry, float deltaTime)
		{
			registry.View().Exclude<Dead>().ForEachExtra(deltaTime,
				(ref Velocity velocity, ref Position position, float deltaTime) =>
				{
					position.Value += velocity.Value * deltaTime;
				});
		}
	}
}
