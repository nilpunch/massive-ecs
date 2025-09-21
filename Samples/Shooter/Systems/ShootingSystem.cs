namespace Massive.Samples.Shooter
{
	public static class ShootingSystem
	{
		public static void Update(World world, float deltaTime)
		{
			world.None<Dead>().ForEach((world, deltaTime),
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
					bullet.Set(position);
					bullet.Set(new Bullet() { Damage = weapon.BulletDamage, Lifetime = weapon.BulletLifetime, Owner = world.GetEntifier(id) });
					bullet.Set(new Velocity() { Value = weapon.ShootingDirection * weapon.BulletSpeed });
					bullet.Set(new CircleCollider() { Radius = 0.1f });
					bullet.Set(new VelocityDamper() { DampingFactor = 0.05f });

					character.Bullets.In(world).Add(bullet.Entifier);
				});
		}
	}
}
