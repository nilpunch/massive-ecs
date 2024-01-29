using UnityEngine;

namespace Massive.Samples.Shooter
{
	public class ShootingCharacter : MonoBehaviour, IWorldComponent<CharacterState>
	{
		[SerializeField] private float _cooldown = 0.2f;
		[SerializeField] private float _bulletVelocity = 1f;
		[SerializeField] private float _bulletDamage = 1f;
		[SerializeField] private float _bulletLifetime = 2f;

		public void UpdateState(World world, ref CharacterState state)
		{
			ref WeaponState weaponState = ref state.Weapon;

			weaponState.Cooldown -= world.DeltaTime;
			if (weaponState.Cooldown > 0)
			{
				return;
			}

			weaponState.Cooldown = _cooldown;

			BulletState createBullet = new BulletState
			{
				Transform = state.Transform,
				Velocity = Vector3.up * _bulletVelocity,
				Lifetime = _bulletLifetime,
				Damage = _bulletDamage
			};

			world.Bullets.Create();
		}
	}
}