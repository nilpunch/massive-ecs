using System;
using System.Numerics;

namespace Massive.Samples.Shooter
{
	public class Shooter
	{
		public World World { get; } = new World();

		public Action<World, float> Systems { get; } = (_, _) => { };

		public Shooter()
		{
			Systems += BulletLifetimeSystem.Update;
			Systems += CharacterRespawnSystem.Update;
			Systems += DamageSystem.Update;
			Systems += DelayedBulletDeathSystem.Update;
			Systems += MovementSystem.Update;
			Systems += ShootingSystem.Update;
			Systems += VelocityDampingSystem.Update;
		}

		public void CreateCharactersInTwoOppositeRows(Vector2 startPosition, Vector2 offset, Vector2 axis, int amount)
		{
			var random = new Random(0);

			for (int i = 0; i < amount; i++)
			{
				CreateCharacter(startPosition - offset + axis * i, offset, 5f + (float)random.Next() / int.MaxValue * 2f);
				CreateCharacter(startPosition + offset + axis * i, -offset, 4f + (float)random.Next() / int.MaxValue * 2f);
			}
		}

		public void CreateCharacter(Vector2 position, Vector2 direction, float bulletsPerSecond)
		{
			var id = World.Create();
			World.Set(id, new Character(maxHealth: 20, World.AllocAutoList<Entity>(id)));
			World.Set(id, new Position() { Value = position });
			World.Set(id, new Weapon()
			{
				BulletsPerSecond = bulletsPerSecond, ShootingDirection = Vector2.Normalize(direction),
				BulletDamage = 1, BulletSpeed = 5f, BulletLifetime = 5f
			});
			World.Set(id, new CircleCollider() { Radius = 0.25f });
		}
	}
}
