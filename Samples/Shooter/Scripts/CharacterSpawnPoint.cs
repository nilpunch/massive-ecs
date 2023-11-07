using System;
using UnityEngine;

namespace Massive.Samples.Shooter
{
    public class CharacterSpawnPoint : MonoBehaviour
    {
        [SerializeField] private float _health = 10f;

        private void Start()
        {
            Destroy(gameObject);
        }

        public CharacterState GetState()
        {
            return new CharacterState()
            {
                Transform = new TransformState()
                {
                    Position = transform.position, Rotation = transform.rotation,
                },
                Health = _health
            };
        }
    }
}
