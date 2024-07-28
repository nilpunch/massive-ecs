namespace Massive.Samples.Shooter
{
	/// <summary>
	/// Respawns dead characters with full health after some time.
	/// </summary>
	public static class CharacterRespawnSystem
	{
		public static void Update(IRegistry registry, float deltaTime)
		{
			registry.View().ForEachExtra((registry, deltaTime),
				static (int characterId, ref Dead dead, ref Character character, (IRegistry Registry, float DeltaTime) args) =>
				{
					var (registry, deltaTime) = args;

					dead.ElapsedTimeSinceDeath += deltaTime;

					if (dead.ElapsedTimeSinceDeath > 3f)
					{
						registry.Unassign<Dead>(characterId);
						character.Health = character.MaxHealth;
					}
				});
		}
	}
}
