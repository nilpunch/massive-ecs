using System;
using Massive.Samples.Misc;
using UnityEngine;

namespace Massive.Samples.Shooter
{
    public class Character : MonoBehaviour, IEntity<CharacterState>
    {
        [SerializeField] private Weapon _weapon;
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private Collider[] _colliders;

        private bool _isEntityActive;
        private TransformState _visualTransformState;

        public void Awake()
        {
            _weapon.Inject(ShooterWorld.WorldTime, ShooterWorld.BulletWorld);
        }

        public void UpdateState(ref CharacterState state)
        {
            if (state.Health <= 0)
            {
                Disable();
                return;
            }
            else
            {
                Enable();
            }

            _weapon.UpdateState(ref state);

            _visualTransformState = state.Transform;
        }

        private void LateUpdate()
        {
            transform.position = _visualTransformState.Position;
            transform.rotation = _visualTransformState.Rotation;
        }

        public void Enable()
        {
            if (_isEntityActive)
            {
                return;
            }

            _isEntityActive = true;

            SetActive(true);
        }

        public void Disable()
        {
            if (!_isEntityActive)
            {
                return;
            }

            _isEntityActive = false;

            SetActive(false);
        }

        public void SetActive(bool active)
        {
            foreach (var renderer in _renderers)
            {
                renderer.enabled = active;
            }
            foreach (var collider in _colliders)
            {
                collider.enabled = active;
            }
        }
    }
}
