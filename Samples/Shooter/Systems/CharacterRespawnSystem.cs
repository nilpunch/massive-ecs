namespace Massive.Samples.Shooter
{
	/// <summary>
	/// Respawns dead characters with full health after some time.
	/// </summary>
	public class CharacterRespawnSystem : SystemBase, IUpdate
	{
		public void Update(float deltaTime)
		{
			World.ForEach((World, deltaTime),
				static (int characterId, ref Dead dead, ref Character character, (World World, float DeltaTime) args) =>
				{
					var (world, deltaTime) = args;

					dead.ElapsedTimeSinceDeath += deltaTime;

					if (dead.ElapsedTimeSinceDeath > 3f)
					{
						world.Remove<Dead>(characterId);
						character.Health = character.MaxHealth;
					}
				});
		}
	}
}
