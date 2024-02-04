using System;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public class ParticleSpawnPoint : MonoBehaviour
	{
		public float Drag = 0.5f;
		public float Radius = 0.5f;
		public Vector3 Position => transform.position;

		private void Start()
		{
			Destroy(gameObject);
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawSphere(Position, Radius);
		}
	}
}