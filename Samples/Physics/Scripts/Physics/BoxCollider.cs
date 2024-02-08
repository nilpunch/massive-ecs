using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public struct BoxCollider
	{
		public readonly int RigidbodyId;

		public Vector3 Size;
		public Vector3 HalfSize;

		public Vector3 LocalPosition;
		public Vector3 WorldPosition;
		public Quaternion WorldRotation;

		public BoxCollider(int rigidbodyId, Vector3 size)
		{
			RigidbodyId = rigidbodyId;
			Size = size;
			HalfSize = size * 0.5f;
			
			LocalPosition = Vector3.zero;
			WorldPosition = Vector3.zero;
			WorldRotation = Quaternion.identity;
		}

		public static void UpdateWorldPositions(Massive<Rigidbody> bodies, Massive<BoxCollider> colliders)
		{
			var aliveColliders = colliders.AliveData;
			for (int i = 0; i < aliveColliders.Length; i++)
			{
				var collider = aliveColliders[i];
				aliveColliders[i].WorldPosition = collider.LocalPosition + bodies.Get(collider.RigidbodyId).Position;
				aliveColliders[i].WorldRotation = bodies.Get(collider.RigidbodyId).Rotation;
			}
		}
	}
}