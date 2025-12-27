namespace Massive.Samples.Shooter
{
	/// <summary>
	/// Move entities according to their velocities.
	/// </summary>
	public class MovementSystem : WorldSystem, IUpdate
	{
		public void Update(float deltaTime)
		{
			World.Exclude<Dead>().ForEach(deltaTime,
				static (ref Velocity velocity, ref Position position, float deltaTime) =>
				{
					position.Value += velocity.Value * deltaTime;
				});
		}
	}
}
