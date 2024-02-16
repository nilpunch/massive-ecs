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
		public Quaternion LocalRotation;
		
		public Vector3 WorldPosition;
		public Quaternion WorldRotation;

		public SphereCollider(int rigidbodyId, float radius, Vector3 localPosition, Quaternion localRotation)
		{
			RigidbodyId = rigidbodyId;
			Radius = radius;
			
			LocalPosition = localPosition;
			LocalRotation = localRotation;
			WorldPosition = Vector3.zero;
			WorldRotation = Quaternion.identity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 TransformFromLocalToWorld(Vector3 localPosition)
		{
			return WorldPosition + localPosition;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix4x4 CalculateLocalMoi(float mass)
		{
			float i = (2f / 5f) * mass * Radius * Radius;

			Matrix4x4 inertiaTensor = new Matrix4x4();
			inertiaTensor.SetRow(0, new Vector4(i, 0f, 0f, 0f));
			inertiaTensor.SetRow(1, new Vector4(0f, i, 0f, 0f));
			inertiaTensor.SetRow(2, new Vector4(0f, 0f, i, 0f));
			inertiaTensor.SetRow(3, new Vector4(0f, 0f, 0f, 1f)); // The last row is not used for MOI calculations
			
			return Rigidbody.TransformMoi(inertiaTensor, LocalPosition, Quaternion.identity, mass);
		}
		
		public static void UpdateWorldPositions(Massive<Rigidbody> bodies, Massive<SphereCollider> colliders)
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