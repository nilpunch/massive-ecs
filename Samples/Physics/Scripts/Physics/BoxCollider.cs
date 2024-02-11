using System.Runtime.CompilerServices;
using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public struct BoxCollider
	{
		public readonly int RigidbodyId;

		public Vector3 Size;
		public Vector3 HalfSize;

		public Vector3 LocalPosition;
		public Quaternion LocalRotation;
		
		public Vector3 WorldPosition;
		public Quaternion WorldRotation;

		public BoxCollider(int rigidbodyId, Vector3 size, Vector3 localPosition, Quaternion localRotation)
		{
			RigidbodyId = rigidbodyId;
			Size = size;
			HalfSize = size * 0.5f;
			
			LocalPosition = localPosition;
			LocalRotation = localRotation;
			
			WorldPosition = Vector3.zero;
			WorldRotation = Quaternion.identity;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 TransformFromLocalToWorld(Vector3 localPosition)
		{
			return WorldPosition + WorldRotation * localPosition;
		}

		public static void UpdateWorldPositions(Massive<Rigidbody> bodies, Massive<BoxCollider> colliders)
		{
			var aliveColliders = colliders.AliveData;
			for (int i = 0; i < aliveColliders.Length; i++)
			{
				ref var collider = ref aliveColliders[i];
				var body = bodies.Get(collider.RigidbodyId);
				collider.WorldPosition = body.GetWorldCenterOfMass() + body.Rotation * collider.LocalPosition;
				collider.WorldRotation = body.Rotation * collider.LocalRotation;
			}
		}
	}
}