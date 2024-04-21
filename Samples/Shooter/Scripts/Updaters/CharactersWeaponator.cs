using UnityEngine;

namespace Massive.Samples.Shooter
{
	public class CharactersWeaponator : WorldUpdater
	{
		[SerializeField] private float _cooldown = 0.2f;
		[SerializeField] private float _bulletVelocity = 1f;
		[SerializeField] private float _bulletDamage = 1f;
		[SerializeField] private float _bulletLifetime = 2f;

		private IRegistry _registry;
		private GroupView<CharacterState, WeaponState> _characters;

		public override void Init(IRegistry registry)
		{
			_registry = registry;
			_characters = new GroupView<CharacterState, WeaponState>(registry, registry.Group(registry.Many<CharacterState, WeaponState>()));
		}

		public override void UpdateWorld(float deltaTime)
		{
			_characters.ForEachExtra((_registry, deltaTime, _cooldown, _bulletVelocity, _bulletDamage, _bulletLifetime),
				(int entity, ref CharacterState characterState, ref WeaponState weaponState,
					(IRegistry Registry, float DeltaTime, float DefaultCooldown, float BulletVelocity, float BulletDamage, float BulletLifetime) inner) =>
				{
					weaponState.Cooldown -= inner.DeltaTime;
					if (weaponState.Cooldown > 0)
					{
						return;
					}

					weaponState.Cooldown = inner.DefaultCooldown;

					inner.Registry.Create(new BulletState
					{
						Transform = characterState.Transform,
						Velocity = characterState.Transform.Rotation * Vector3.up * inner.BulletVelocity,
						Lifetime = inner.BulletLifetime,
						Damage = inner.BulletDamage
					});
				});
		}
	}
}