namespace Massive.Samples.Shooter
{
	public static class ShootingSystem
	{
		public static void Update(World world, float deltaTime)
		{
			world.View().Exclude<Dead>().ForEach((world, deltaTime),
				static (int id, ref Character character, ref Weapon weapon, ref Position position, (World World, float DeltaTime) args) =>
				{
					var (world, deltaTime) = args;

					weapon.Charge += weapon.BulletsPerSecond * deltaTime;

					if (weapon.Charge < 1f)
					{
						return;
					}

					weapon.Charge -= 1f;

					var bullet = world.CreateEntity();
					world.Set(bullet, position);
					world.Set(bullet, new Bullet() { Damage = weapon.BulletDamage, Lifetime = weapon.BulletLifetime, Owner = world.GetEntity(id) });
					world.Set(bullet, new Velocity() { Value = weapon.ShootingDirection * weapon.BulletSpeed });
					world.Set(bullet, new CircleCollider() { Radius = 0.1f });
					world.Set(bullet, new VelocityDamper() { DampingFactor = 0.05f });

					character.Bullets.In(world).Add(bullet);
				});
		}
	}
}
