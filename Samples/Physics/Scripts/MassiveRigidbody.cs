using System;
using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public class MassiveRigidbody : MonoBehaviour
	{
		[SerializeField] private float _mass = 1f;
		[SerializeField] private float _inertia = 1f;
		[SerializeField] private float _restitution = 1f;
		[SerializeField] private float _friction = 1f;
		[SerializeField] private bool _static = false;
		[SerializeField] private Vector3 _startImpulse;
		[SerializeField] private Vector3 _startImpulsePoint;

		public void Spawn(Massive<Rigidbody> bodies, Massive<SphereCollider> spheres, Massive<BoxCollider> boxes)
		{
			int bodyId = bodies.Create(new Rigidbody(transform.position, transform.rotation, _mass, _restitution, _friction, _inertia, _static));

			foreach (var sphereCollider in GetComponentsInChildren<MassiveSphereCollider>())
			{
				spheres.Create(new SphereCollider(bodyId, sphereCollider.Radius,
					transform.InverseTransformPoint(sphereCollider.transform.position),
					Quaternion.Inverse(transform.rotation) * sphereCollider.transform.rotation));
			}
			
			bodies.Get(bodyId).ApplyImpulseAtPoint(_startImpulse, _startImpulsePoint);

			foreach (var boxCollider in GetComponentsInChildren<MassiveBoxCollider>())
			{
				boxes.Create(new BoxCollider(bodyId, boxCollider.Size,
					transform.InverseTransformPoint(boxCollider.transform.position),
					Quaternion.Inverse(transform.rotation) * boxCollider.transform.rotation));
			}
		}

		private void Start()
		{
			Destroy(gameObject);
		}
	}
}