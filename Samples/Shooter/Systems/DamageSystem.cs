namespace Massive.Samples.Shooter
{
	public class DamageSystem : SystemBase, IUpdate
	{
		public void Update(float deltaTime)
		{
			var characters = World.DataSet<Character>();
			var bullets = World.DataSet<Bullet>();
			var colliders = World.DataSet<CircleCollider>();
			var positions = World.DataSet<Position>();

			foreach (var characterId in World.Include<Character>().Exclude<Dead>())
			{
				ref var character = ref characters.Get(characterId);

				foreach (var bulletId in World.Include<Bullet>().Exclude<Dead>())
				{
					ref var bullet = ref bullets.Get(bulletId);

					// Don't collide a character with its own bullet.
					if (bullet.Owner == World.GetEntifier(characterId))
					{
						continue;
					}

					if (IsCollided(bulletId, characterId))
					{
						World.Set(bulletId, new Dead());

						character.Health -= bullet.Damage;
						if (character.Health <= 0)
						{
							World.Set(characterId, new Dead());
							break;
						}
					}
				}
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
