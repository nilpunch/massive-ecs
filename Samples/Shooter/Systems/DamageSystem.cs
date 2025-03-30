﻿namespace Massive.Samples.Shooter
{
	public static class DamageSystem
	{
		public static void Update(World world, float deltaTime)
		{
			var characters = world.Data<Character>();
			var bullets = world.Data<Bullet>();
			var colliders = world.Data<CircleCollider>();
			var positions = world.Data<Position>();

			foreach (var characterId in world.View().Filter<Include<Character>, Exclude<Dead>>())
			{
				ref var character = ref characters.Get(characterId);

				foreach (var bulletId in world.View().Filter<Include<Bullet>, Exclude<Dead>>())
				{
					ref var bullet = ref bullets.Get(bulletId);

					// Don't collide a character with its own bullet.
					if (bullet.Owner == world.GetEntity(characterId))
					{
						continue;
					}

					if (IsCollided(bulletId, characterId))
					{
						world.Set(bulletId, new Dead());

						character.Health -= bullet.Damage;
						if (character.Health <= 0)
						{
							world.Set(characterId, new Dead());
							DestroyCharacterBullets(characterId);
							break;
						}
					}
				}
			}

			void DestroyCharacterBullets(int characterId)
			{
				world.View().Exclude<Dead>().ForEachExtra((characterId, world),
					static (int bulletId, ref Bullet bullet, (int CharacterId, World World) args) =>
					{
						var (characterId, world) = args;

						if (bullet.Owner == world.GetEntity(characterId))
						{
							world.Add<Dead>(bulletId);
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
