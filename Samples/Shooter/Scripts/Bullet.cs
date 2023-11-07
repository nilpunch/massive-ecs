using System;
using Massive.Samples.Misc;
using UnityEngine;

namespace Massive.Samples.Shooter
{
    public class Bullet : MonoBehaviour, IEntity<BulletState>
    {
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private Collider[] _colliders;

        private bool _isEntityActive;
        private WorldTime _worldTime;
        private TransformState _visualTransformState;

        public void Awake()
        {
            _worldTime = ShooterWorld.WorldTime;
        }

        public void UpdateState(ref BulletState state)
        {
            if (state.IsDestroyed)
            {
                Disable();
                return;
            }
            else
            {
                Enable();
            }

            state.Lifetime -= _worldTime.DeltaTime;

            state.Transform.Position += state.Velocity * _worldTime.DeltaTime;

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
