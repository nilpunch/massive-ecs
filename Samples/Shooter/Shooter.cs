using System;
using System.Numerics;

namespace Massive.Samples.Shooter
{
	public class Shooter
	{
		public IRegistry Registry { get; } = new Registry();

		public Action<IRegistry, float> Systems { get; } = (_, _) => { };

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
				CreateCharacter(startPosition - offset + axis * i, offset, 0.2f + (float)random.Next() / int.MaxValue * 0.1f);
				CreateCharacter(startPosition + offset + axis * i, -offset, 0.3f + (float)random.Next() / int.MaxValue * 0.1f);
			}
		}

		public void CreateCharacter(Vector2 position, Vector2 direction, float bulletsPerSecond)
		{
			var id = Registry.Create();
			Registry.Assign(id, new Character(maxHealth: 20));
			Registry.Assign(id, new Position() { Value = position });
			Registry.Assign(id, new Weapon()
			{
				BulletsPerSecond = bulletsPerSecond, ShootingDirection = Vector2.Normalize(direction),
				BulletDamage = 1, BulletSpeed = 5f, BulletLifetime = 5f
			});
			Registry.Assign(id, new CircleCollider() { Radius = 0.25f });
		}
	}
}
