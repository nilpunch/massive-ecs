namespace Massive.Samples.Shooter
{
	public static class DamageSystem
	{
		public static void Update(IRegistry registry, float deltaTime)
		{
			var characters = registry.Components<Character>();
			var bullets = registry.Components<Bullet>();
			var colliders = registry.Components<CircleCollider>();
			var positions = registry.Components<Position>();

			foreach (var characterId in registry.View().Filter<Include<Character>, Exclude<Dead>>())
			{
				ref var character = ref characters.Get(characterId);

				foreach (var bulletId in registry.View().Filter<Include<Bullet>, Exclude<Dead>>())
				{
					ref var bullet = ref bullets.Get(bulletId);

					// Don't collide a character with its own bullet
					if (bullet.Owner == registry.GetEntity(characterId))
					{
						continue;
					}

					if (IsCollided(bulletId, characterId))
					{
						registry.Assign<Dead>(bulletId);

						character.Health -= bullet.Damage;
						if (character.Health <= 0)
						{
							registry.Assign<Dead>(characterId);
							DestroyCharacterBullets(characterId);
							break;
						}
					}
				}
			}

			void DestroyCharacterBullets(int characterId)
			{
				registry.View().Exclude<Dead>().ForEachExtra(registry,
					(int bulletId, ref Bullet bullet, IRegistry registry) =>
				{
					if (bullet.Owner == registry.GetEntity(characterId))
					{
						registry.Assign<Dead>(bulletId);
					}
				});
			}
			
			bool IsCollided(int firstId, int secondId)
			{
				ref var firstPosition = ref positions.Get(firstId);
				ref var firstCollider = ref colliders.Get(firstId);
				ref var secondPosition = ref positions.Get(secondId);
				ref var secondCollider = ref colliders.Get(secondId);

				return CircleCollider.IsCollided(
					firstPosition.Value, firstCollider.Radius,
					secondPosition.Value, secondCollider.Radius);
			}
		}
	}
}
