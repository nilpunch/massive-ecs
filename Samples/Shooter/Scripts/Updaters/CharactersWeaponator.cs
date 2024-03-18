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
		private View<CharacterState, WeaponState> _characters;

		public override void Init(IRegistry registry)
		{
			_registry = registry;
			_characters = new View<CharacterState, WeaponState>(registry);
		}

		public override void UpdateWorld(float deltaTime)
		{
			_characters.ForEachExtra(deltaTime, (int entity, ref CharacterState characterState, ref WeaponState weaponState, float innerDeltaTime) =>
			{
				weaponState.Cooldown -= innerDeltaTime;
				if (weaponState.Cooldown > 0)
				{
					return;
				}

				weaponState.Cooldown = _cooldown;

				_registry.Create(new BulletState
				{
					Transform = characterState.Transform,
					Velocity = characterState.Transform.Rotation * Vector3.up * _bulletVelocity,
					Lifetime = _bulletLifetime,
					Damage = _bulletDamage
				});
			});
		}
	}
}