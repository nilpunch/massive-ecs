namespace Massive.Samples.Shooter
{
	/// <summary>
	/// Respawns dead characters with full health after some time.
	/// </summary>
	public static class CharacterRespawnSystem
	{
		public static void Update(World world, float deltaTime)
		{
			world.View().ForEach((world, deltaTime),
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
