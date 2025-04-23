namespace Massive.Samples.Shooter
{
	public static class ShootingSystem
	{
		public static void Update(World world, float deltaTime)
		{
			world.View().Exclude<Dead>().ForEach((world, deltaTime),
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
					world.Set(bulletId, position);
					world.Set(bulletId, new Bullet() { Damage = weapon.BulletDamage, Lifetime = weapon.BulletLifetime, Owner = world.GetEntity(id) });
					world.Set(bulletId, new Velocity() { Value = weapon.ShootingDirection * weapon.BulletSpeed });
					world.Set(bulletId, new CircleCollider() { Radius = 0.1f });
					world.Set(bulletId, new VelocityDamper() { DampingFactor = 0.05f });
				});
		}
	}
}
