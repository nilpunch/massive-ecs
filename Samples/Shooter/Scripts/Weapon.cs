using Massive.Samples.Misc;
using UnityEngine;

namespace Massive.Samples.Shooter
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private float _cooldown = 0.2f;
        [SerializeField] private float _bulletVelocity = 1f;
        [SerializeField] private float _bulletDamage = 1f;
        [SerializeField] private float _bulletLifetime = 2f;

        private WorldTime _worldTime;
        private WorldState<BulletState> _bulletWorld;

        public void Inject(WorldTime worldTime, WorldState<BulletState> bulletWorld)
        {
            _bulletWorld = bulletWorld;
            _worldTime = worldTime;
        }

        public void UpdateState(ref CharacterState characterState)
        {
            ref WeaponState weaponState = ref characterState.Weapon;

            weaponState.Cooldown -= _worldTime.DeltaTime;
            if (weaponState.Cooldown > 0)
            {
                return;
            }
            weaponState.Cooldown = _cooldown;

            BulletState createBullet = new BulletState
            {
                Transform = characterState.Transform,
                Velocity = Vector3.up * _bulletVelocity,
                Lifetime = _bulletLifetime,
                Damage = _bulletDamage
            };

            // Check if there is any destroyed bullet
            foreach (ref BulletState bulletState in _bulletWorld.GetAll())
            {
                if (bulletState.IsDestroyed)
                {
                    bulletState = createBullet;
                    return;
                }
            }

            // Reserve new bullet
            if (_bulletWorld.CanReserveState)
            {
                _bulletWorld.Reserve(createBullet);
            }
        }
    }
}
