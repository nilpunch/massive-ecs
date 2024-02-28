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

		private IRegistry _registry;
		private IDataSet<CharacterState> _characters;
		private IDataSet<WeaponState> _weapons;

		public override void Init(IRegistry registry)
		{
			_registry = registry;
			_characters = registry.Components<CharacterState>();
			_weapons = registry.Components<WeaponState>();
		}

		public override void UpdateWorld(float deltaTime)
		{
			var ids1 = _characters.AliveIds;
			var data1 = _characters.AliveData;
			var data2 = _weapons.AliveData;

			for (int dense1 = 0; dense1 < ids1.Length; dense1++)
			{
				int id = ids1[dense1];
				if (_weapons.TryGetDense(id, out var dense2))
				{
					ref var characterState = ref data1[dense1];
					ref var weaponState = ref data2[dense2];
					
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
}