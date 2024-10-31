namespace Massive.Samples.Shooter
{
	/// <summary>
	/// Respawns dead characters with full health after some time.
	/// </summary>
	public static class CharacterRespawnSystem
	{
		public static void Update(Registry registry, float deltaTime)
		{
			foreach (var characterId in registry.View().Filter<Include<Dead, Character>>())
			{
				ref var dead = ref registry.Get<Dead>(characterId);
				ref var character = ref registry.Get<Character>(characterId);
				
				dead.ElapsedTimeSinceDeath += deltaTime;
				
				if (dead.ElapsedTimeSinceDeath > 3f)
				{
					registry.Unassign<Dead>(characterId);
					character.Health = character.MaxHealth;
				}
			}
		}
	}
}
