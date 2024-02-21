using UnityEngine;

namespace Massive.Samples.Physics
{
	public class DebugCollisionDetection : MonoBehaviour
	{
		[SerializeField] private Vector3 _size;
		[Space]
		[SerializeField] private UnityEngine.Transform _sphere;
		[SerializeField] private float _sphereRadius;

		private void OnDrawGizmos()
		{
			if (_sphere == null)
				return;

			SphereCollider sphereCollider = new SphereCollider(0, _sphereRadius, new Transformation(), new PhysicMaterial())
			{
				World = new Transformation(_sphere.position, _sphere.rotation)
			};
			
			BoxCollider boxCollider = new BoxCollider(0, _size, new Transformation(), new PhysicMaterial())
			{
				World = new Transformation(transform.position, transform.rotation)
			};

			SphereCollider sphereCollider2 = new SphereCollider(0, _sphereRadius, new Transformation(), new PhysicMaterial())
			{
				World = new Transformation(transform.position, transform.rotation)
			};

			Gizmos.DrawSphere(sphereCollider.World.Position, _sphereRadius);
			Gizmos.DrawSphere(sphereCollider2.World.Position, _sphereRadius);

			// var orig = Gizmos.matrix;
			// Gizmos.matrix = transform.localToWorldMatrix;
			// Gizmos.DrawCube(Vector3.zero, _size);
			// Gizmos.matrix = orig;

			ColliderContact colliderContact = new ColliderContact();
			
			// CollisionTester.SphereVsBox(ref sphereCollider, ref boxCollider, boxCollider.WorldPosition - sphereCollider.WorldPosition, boxCollider.WorldRotation, ref contact);
			CollisionTester.SphereVsSphere(ref sphereCollider, ref sphereCollider2, sphereCollider2.World.Position - sphereCollider.World.Position, ref colliderContact);

			if (colliderContact.Depth > 0f)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(sphereCollider.World.Position + colliderContact.OffsetFromColliderA, 0.1f);
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(sphereCollider.World.Position + colliderContact.OffsetFromColliderA, sphereCollider.World.Position + colliderContact.OffsetFromColliderA + colliderContact.Normal * colliderContact.Depth);
			}
		}
	}
}