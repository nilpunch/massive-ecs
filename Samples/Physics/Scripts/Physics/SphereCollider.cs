using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public struct SphereCollider
	{
		public readonly int RigidbodyId;

		public float Radius;

		public Vector3 LocalPosition;
		public Vector3 WorldPosition;
		public Quaternion WorldRotation;

		public SphereCollider(int rigidbodyId, float radius)
		{
			RigidbodyId = rigidbodyId;
			Radius = radius;
			
			LocalPosition = Vector3.zero;
			WorldPosition = Vector3.zero;
			WorldRotation = Quaternion.identity;
		}

		public static void UpdateWorldPositions(Massive<Rigidbody> bodies, Massive<SphereCollider> colliders)
		{
			var aliveColliders = colliders.AliveData;
			for (int i = 0; i < aliveColliders.Length; i++)
			{
				SphereCollider collider = aliveColliders[i];
				aliveColliders[i].WorldPosition = collider.LocalPosition + bodies.Get(collider.RigidbodyId).Position;
				aliveColliders[i].WorldRotation = bodies.Get(collider.RigidbodyId).Rotation;
			}
		}
	}
}