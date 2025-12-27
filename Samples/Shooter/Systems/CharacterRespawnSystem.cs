namespace Massive.Samples.Shooter
{
	/// <summary>
	/// Respawns dead characters with full health after some time.
	/// </summary>
	public class CharacterRespawnSystem : WorldSystem, IUpdate
	{
		public void Update(float deltaTime)
		{
			World.ForEach(deltaTime,
				static (Entity entity, ref Dead dead, ref Character character, float deltaTime) =>
				{
					dead.ElapsedTimeSinceDeath += deltaTime;

					if (dead.ElapsedTimeSinceDeath > 3f)
					{
						entity.Remove<Dead>();
						character.Health = character.MaxHealth;
					}
				});
		}
	}
}
