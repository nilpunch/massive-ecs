﻿namespace Massive.Samples.Shooter
{
	public static class ShootingSystem
	{
		public static void Update(World world, float deltaTime)
		{
			world.View().Exclude<Dead>().ForEachExtra((world: world, deltaTime),
				static (int id, ref Weapon weapon, ref Position position, (World World, float DeltaTime) args) =>
				{
					var (world, deltaTime) = args;

					weapon.Charge += weapon.BulletsPerSecond * deltaTime;

					if (weapon.Charge < 1f)
					{
						return;
					}

					weapon.Charge -= 1f;

					var bulletId = world.Create();
					world.Assign(bulletId, position);
					world.Assign(bulletId, new Bullet() { Damage = weapon.BulletDamage, Lifetime = weapon.BulletLifetime, Owner = world.GetEntity(id) });
					world.Assign(bulletId, new Velocity() { Value = weapon.ShootingDirection * weapon.BulletSpeed });
					world.Assign(bulletId, new CircleCollider() { Radius = 0.1f });
					world.Assign(bulletId, new VelocityDamper() { DampingFactor = 0.05f });
				});
		}
	}
}
