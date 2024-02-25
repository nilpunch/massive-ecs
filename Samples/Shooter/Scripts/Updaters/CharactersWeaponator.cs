using Massive.ECS;
using UnityEngine;

namespace Massive.Samples.Shooter
{
	public class CharactersWeaponator : WorldUpdater
	{
		[SerializeField] private float _cooldown = 0.2f;
		[SerializeField] private float _bulletVelocity = 1f;
		[SerializeField] private float _bulletDamage = 1f;
		[SerializeField] private float _bulletLifetime = 2f;

		private Registry _registry;
		private View<CharacterState, WeaponState> _weapons;

		public override void Init(Registry registry)
		{
			_registry = registry;
			_weapons = registry.View<CharacterState, WeaponState>();
		}

		public override void UpdateWorld(float deltaTime)
		{
			_weapons.ForEach((ref CharacterState characterState, ref WeaponState weaponState) =>
			{
				weaponState.Cooldown -= deltaTime;
				if (weaponState.Cooldown > 0)
				{
					return;
				}

				weaponState.Cooldown = _cooldown;

				BulletState bulletState = new BulletState
				{
					Transform = characterState.Transform,
					Velocity = characterState.Transform.Rotation * Vector3.up * _bulletVelocity,
					Lifetime = _bulletLifetime,
					Damage = _bulletDamage
				};

				_registry.CreateEntity(bulletState);
			});
		}
	}
}