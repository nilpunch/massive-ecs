namespace Massive.Samples.Shooter
{
	/// <summary>
	/// Move entities according to their velocities.
	/// </summary>
	public static class MovementSystem
	{
		public static void Update(World world, float deltaTime)
		{
			world.Exclude<Dead>().ForEach(deltaTime,
				static (ref Velocity velocity, ref Position position, float deltaTime) =>
				{
					position.Value += velocity.Value * deltaTime;
				});
		}
	}
}
