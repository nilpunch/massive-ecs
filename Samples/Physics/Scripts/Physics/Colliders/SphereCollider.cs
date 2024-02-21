using System.Runtime.CompilerServices;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public struct SphereCollider
	{
		public readonly int RigidbodyId;

		public PhysicMaterial Material;
		public Transformation Local;
		public Transformation World;
		
		public float Radius;

		public SphereCollider(int rigidbodyId, float radius, Transformation local, PhysicMaterial material)
		{
			RigidbodyId = rigidbodyId;
			Radius = radius;
			
			Material = material;
			Local = local;
			World = local;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Mass()
		{
			float volume = 4f / 3f * Mathf.PI * Radius * Radius * Radius;
			return Material.Density * volume;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix4x4 LocalInertiaTensor()
		{
			float volume = 4f / 3f * Mathf.PI * Radius * Radius * Radius;
			float mass = Material.Density * volume;
			
			float i = 2f / 5f * mass * Radius * Radius;

			Matrix4x4 inertiaTensor = new Matrix4x4();
			inertiaTensor.SetRow(0, new Vector4(i, 0f, 0f, 0f));
			inertiaTensor.SetRow(1, new Vector4(0f, i, 0f, 0f));
			inertiaTensor.SetRow(2, new Vector4(0f, 0f, i, 0f));
			inertiaTensor.SetRow(3, new Vector4(0f, 0f, 0f, 1f)); // The last row is not used for MOI calculations
			
			return Rigidbody.TransformInertiaTensor(inertiaTensor, Local.Position, Quaternion.identity, mass);
		}
		
		public static void UpdateWorldPositions(MassiveDataSet<Rigidbody> bodies, MassiveDataSet<SphereCollider> colliders)
		{
			var aliveColliders = colliders.AliveData;
			for (int i = 0; i < aliveColliders.Length; i++)
			{
				ref var collider = ref aliveColliders[i];
				collider.World = collider.Local.LocalToWorld(bodies.Get(collider.RigidbodyId).WorldCenterOfMass);
			}
		}
	}
}