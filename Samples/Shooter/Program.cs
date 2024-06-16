using System.Numerics;

namespace Massive.Samples.Shooter
{
	public class ShooterGame
	{
		public IRegistry Registry { get; } = new Registry();

		public ShooterGame()
		{
		}

		public void CreateCharacter(Vector2 position, Vector2 direction)
		{
			var id = Registry.Create();
			Registry.Assign(id, new Character(maxHealth: 5));
			Registry.Assign(id, new Position() { Value = position });
			Registry.Assign(id, new Weapon()
			{
				ShootingDelay = 0.2f, ShootingDirection = Vector2.Normalize(direction),
				BulletDamage = 1, BulletSpeed = 5f, BulletLifetime = 5f
			});
			Registry.Assign(id, new CircleCollider() { Radius = 0.25f });
		}
	}
}
