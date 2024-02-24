using UnityEngine;

namespace Massive.Samples.Shooter
{
	public class CharactersUpdater : WorldUpdater
	{
		[SerializeField] private float _cooldown = 0.2f;
		[SerializeField] private float _bulletVelocity = 1f;
		[SerializeField] private float _bulletDamage = 1f;
		[SerializeField] private float _bulletLifetime = 2f;

		[Header("Rotation")] [SerializeField] private float _rotation = 2f;

		public override void UpdateWorld(in WorldFrame worldFrame)
		{
			var characters = worldFrame.Characters.AliveData;

			for (int i = 0; i < worldFrame.Characters.AliveCount; i++)
			{
				ref CharacterState characterState = ref characters[i];
				ref WeaponState weaponState = ref characterState.Weapon;

				characterState.Transform.Rotation *= Quaternion.AngleAxis(_rotation * worldFrame.DeltaTime, Vector3.forward);

				weaponState.Cooldown -= worldFrame.DeltaTime;
				if (weaponState.Cooldown > 0)
				{
					continue;
				}

				weaponState.Cooldown = _cooldown;

				BulletState createBullet = new BulletState
				{
					Transform = characterState.Transform,
					Velocity = characterState.Transform.Rotation * Vector3.up * _bulletVelocity,
					Lifetime = _bulletLifetime,
					Damage = _bulletDamage
				};

				int temp = worldFrame.Bullets.Create(createBullet).Id;
			}
		}
	}
}