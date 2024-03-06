using System.Runtime.CompilerServices;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public struct PhysicsBoxCollider : ISupportMappable
	{
		public readonly int RigidbodyId;

		public PhysicMaterial Material;
		public Transformation Local;
		public Transformation World;

		public Vector3 Size;
		public Vector3 HalfSize;

		public PhysicsBoxCollider(int rigidbodyId, Vector3 size, Transformation local, PhysicMaterial material)
		{
			RigidbodyId = rigidbodyId;
			Size = size;
			Material = material;
			HalfSize = size * 0.5f;

			Local = local;
			World = local;
		}

		Vector3 ISupportMappable.Centre => World.Position;

		Vector3 ISupportMappable.SupportPoint(Vector3 direction)
		{
			Vector3 rotatedDirection = Quaternion.Inverse(World.Rotation) * direction;
			var supportPoint = BoxSupportPoint(Vector3.zero, HalfSize, rotatedDirection);
			var transformedSupportPoint = World.Rotation * supportPoint + World.Position;
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
		public float Mass()
		{
			float volume = Size.x * Size.y * Size.z;
			return Material.Density * volume;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix4x4 LocalInertiaTensor()
		{
			float mass = Mass();

			float ix = (1f / 12f) * mass * (Size.y * Size.y + Size.z * Size.z);
			float iy = (1f / 12f) * mass * (Size.x * Size.x + Size.z * Size.z);
			float iz = (1f / 12f) * mass * (Size.x * Size.x + Size.y * Size.y);

			Matrix4x4 inertiaTensor = new Matrix4x4();
			inertiaTensor.SetRow(0, new Vector4(ix, 0f, 0f, 0f));
			inertiaTensor.SetRow(1, new Vector4(0f, iy, 0f, 0f));
			inertiaTensor.SetRow(2, new Vector4(0f, 0f, iz, 0f));
			inertiaTensor.SetRow(3, new Vector4(0f, 0f, 0f, 1f)); // The last row is not used for MOI calculations

			return PhysicsRigidbody.TransformInertiaTensor(inertiaTensor, Local.Position, Local.Rotation, mass);
		}

		public static void UpdateWorldPositions(IDataSet<PhysicsRigidbody> bodies, IDataSet<PhysicsBoxCollider> colliders)
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