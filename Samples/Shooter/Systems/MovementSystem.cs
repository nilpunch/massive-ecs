namespace Massive.Samples.Shooter
{
	/// <summary>
	/// Move entities according to their velocities.
	/// </summary>
	public static class MovementSystem
	{
		public static void Update(Registry registry, float deltaTime)
		{
			foreach (var id in registry.View().Filter<Include<Velocity, Position>, Exclude<Dead>>())
			{
				ref var velocity = ref registry.Get<Velocity>(id);
				ref var position = ref registry.Get<Position>(id);
				
				position.Value += velocity.Value * deltaTime;
			}
		}
	}
}
