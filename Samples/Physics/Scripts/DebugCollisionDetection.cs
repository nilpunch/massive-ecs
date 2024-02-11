using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public class DebugCollisionDetection : MonoBehaviour
	{
		[SerializeField] private Vector3 _size;
		[Space]
		[SerializeField] private Transform _sphere;
		[SerializeField] private float _sphereRadius;

		private void OnDrawGizmos()
		{
			if (_sphere == null)
				return;

			SphereCollider sphereCollider = new SphereCollider(0, _sphereRadius, Vector3.zero, Quaternion.identity)
			{
				WorldPosition = _sphere.position
			};
			
			BoxCollider boxCollider = new BoxCollider(0, _size, Vector3.zero, Quaternion.identity)
			{
				WorldPosition = transform.position,
				WorldRotation = transform.rotation,
			};

			SphereCollider sphereCollider2 = new SphereCollider(0, _sphereRadius, Vector3.zero, Quaternion.identity)
			{
				WorldPosition = transform.position
			};

			Gizmos.DrawSphere(sphereCollider.WorldPosition, _sphereRadius);
			Gizmos.DrawSphere(sphereCollider2.WorldPosition, _sphereRadius);

			// var orig = Gizmos.matrix;
			// Gizmos.matrix = transform.localToWorldMatrix;
			// Gizmos.DrawCube(Vector3.zero, _size);
			// Gizmos.matrix = orig;

			ColliderContact colliderContact = new ColliderContact();
			
			// CollisionTester.SphereVsBox(ref sphereCollider, ref boxCollider, boxCollider.WorldPosition - sphereCollider.WorldPosition, boxCollider.WorldRotation, ref contact);
			CollisionTester.SphereVsSphere(ref sphereCollider, ref sphereCollider2, sphereCollider2.WorldPosition - sphereCollider.WorldPosition, ref colliderContact);

			if (colliderContact.Depth > 0f)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(sphereCollider.WorldPosition + colliderContact.OffsetFromColliderA, 0.1f);
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(sphereCollider.WorldPosition + colliderContact.OffsetFromColliderA, sphereCollider.WorldPosition + colliderContact.OffsetFromColliderA + colliderContact.Normal * colliderContact.Depth);
			}
		}
	}
}