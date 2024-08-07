namespace Massive.Samples.Shooter
{
	public static class ShootingSystem
	{
		public static void Update(Registry registry, float deltaTime)
		{
			registry.View().Exclude<Dead>().ForEachExtra((registry, deltaTime),
				static (int id, ref Weapon weapon, ref Position position, (Registry Registry, float DeltaTime) args) =>
				{
					var (registry, deltaTime) = args;

					weapon.Charge += weapon.BulletsPerSecond * deltaTime;

					if (weapon.Charge < 1f)
					{
						return;
					}

					weapon.Charge -= 1f;

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
