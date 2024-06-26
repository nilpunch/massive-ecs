﻿namespace Massive.Samples.Shooter
{
	public static class ShootingSystem
	{
		public static void Update(IRegistry registry, float deltaTime)
		{
			registry.View().Exclude<Dead>().ForEachExtra((registry, deltaTime),
				(int id, ref Weapon weapon, ref Position position, (IRegistry Registry, float DeltaTime) args) =>
			{
				weapon.Charge += weapon.BulletsPerSecond * args.DeltaTime;

				if (weapon.Charge < 1f)
				{
					return;
				}

				weapon.Charge -= 1f;

				var registry = args.Registry;
				var bulletId = registry.Create();
				registry.Assign(bulletId, position);
				registry.Assign(bulletId, new Bullet() { Damage = weapon.BulletDamage, Lifetime = weapon.BulletLifetime, Owner = registry.GetEntity(id) });
				registry.Assign(bulletId, new Velocity() { Value = weapon.ShootingDirection * weapon.BulletSpeed });
				registry.Assign(bulletId, new CircleCollider() { Radius = 0.1f });
				registry.Assign(bulletId, new VelocityDamper() { DampingFactor = 0.05f });
			});
		}
	}
}
