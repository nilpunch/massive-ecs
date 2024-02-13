using System.Runtime.CompilerServices;
using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public struct BoxCollider : ISupportMappable
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

		Vector3 ISupportMappable.Centre => WorldPosition;

		Vector3 ISupportMappable.SupportPoint(Vector3 direction)
		{
			Vector3 rotatedDirection = Quaternion.Inverse(WorldRotation) * direction;
			var supportPoint = BoxSupportPoint(Vector3.zero, HalfSize, rotatedDirection);
			var transformedSupportPoint = WorldRotation * supportPoint + WorldPosition;
			return transformedSupportPoint;
		}

		private static Vector3 BoxSupportPoint(Vector3 center, Vector3 extents, Vector3 direction)
		{
			Vector3 signComponents = new Vector3(
				Mathf.Sign(direction.x),
				Mathf.Sign(direction.y),
				Mathf.Sign(direction.z)
			);

			return center + Vector3.Scale(extents, signComponents);
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