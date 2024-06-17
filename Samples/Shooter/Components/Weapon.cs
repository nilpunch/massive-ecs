using System.Numerics;

namespace Massive.Samples.Shooter
{
	public struct Weapon
	{
		public float BulletsPerSecond;
		public float Charge;
		public Vector2 ShootingDirection;

		public float BulletSpeed;
		public float BulletLifetime;
		public int BulletDamage;
	}
}
