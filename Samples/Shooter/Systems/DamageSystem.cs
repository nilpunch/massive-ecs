﻿namespace Massive.Samples.Shooter
{
	public static class DamageSystem
	{
		public static void Update(World world, float deltaTime)
		{
			var characters = world.DataSet<Character>();
			var bullets = world.DataSet<Bullet>();
			var colliders = world.DataSet<CircleCollider>();
			var positions = world.DataSet<Position>();

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
						world.Assign(bulletId, new Dead());

						character.Health -= bullet.Damage;
						if (character.Health <= 0)
						{
							world.Assign(characterId, new Dead());
							DestroyCharacterBullets(characterId);
							break;
						}
					}
				}
			}

			void DestroyCharacterBullets(int characterId)
			{
				world.View().Exclude<Dead>().ForEachExtra((characterId, world: world),
					static (int bulletId, ref Bullet bullet, (int CharacterId, World World) args) =>
					{
						var (characterId, world) = args;

						if (bullet.Owner == world.GetEntity(characterId))
						{
							world.Assign<Dead>(bulletId);
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
