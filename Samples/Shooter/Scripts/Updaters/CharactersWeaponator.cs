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

		private MassiveRegistry _registry;
		private View<CharacterState, WeaponState> _weaponizedCharacters;

		public override void Init(MassiveRegistry registry)
		{
			_registry = registry;
			_weaponizedCharacters = registry.View<CharacterState, WeaponState>();
		}

		public override void UpdateWorld(float deltaTime)
		{
			foreach (var character in _weaponizedCharacters)
			{
				ref CharacterState characterState = ref character.Get<CharacterState>();
				ref WeaponState weaponState = ref character.Get<WeaponState>();
				
				weaponState.Cooldown -= deltaTime;
				if (weaponState.Cooldown > 0)
				{
					continue;
				}

				weaponState.Cooldown = _cooldown;

				_registry.Create(new BulletState
				{
					Transform = characterState.Transform,
					Velocity = characterState.Transform.Rotation * Vector3.up * _bulletVelocity,
					Lifetime = _bulletLifetime,
					Damage = _bulletDamage
				});
			}
		}
	}
}