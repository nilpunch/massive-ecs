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
				(int characterId, ref Dead dead, ref Character character, (IRegistry Registry, float DeltaTime) args) =>
				{
					dead.ElapsedTimeSinceDeath += args.DeltaTime;

					if (dead.ElapsedTimeSinceDeath > 5f)
					{
						args.Registry.Unassign<Dead>(characterId);
						character.Health = character.MaxHealth;
					}
				});
		}
	}
}
