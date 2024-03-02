using UnityEngine;

namespace Massive.Samples.Physics
{
	public class MassiveRigidbody : MonoBehaviour
	{
		[SerializeField] private bool _static = false;
		[SerializeField] private Vector3 _startImpulse;
		[SerializeField] private Vector3 _startImpulsePoint;

		public void Spawn(MassiveDataSet<PhysicsRigidbody> bodies, MassiveDataSet<SphereCollider> spheres, MassiveDataSet<PhysicsBoxCollider> boxes)
		{
			int bodyId = bodies.Create(new PhysicsRigidbody()
			{
				WorldCenterOfMass = new Transformation(transform.position, transform.rotation),
				IsStatic = _static
			}).Id;

			foreach (var sphereCollider in GetComponentsInChildren<MassiveSphereCollider>())
			{
				spheres.Create(new SphereCollider(bodyId, sphereCollider.Radius,
					new Transformation(transform.InverseTransformPoint(sphereCollider.transform.position),
						Quaternion.Inverse(transform.rotation) * sphereCollider.transform.rotation),
					sphereCollider.Material));
			}

			foreach (var boxCollider in GetComponentsInChildren<MassiveBoxCollider>())
			{
				boxes.Create(new PhysicsBoxCollider(bodyId, boxCollider.Size,
					new Transformation(transform.InverseTransformPoint(boxCollider.transform.position), Quaternion.Inverse(transform.rotation) * boxCollider.transform.rotation),
					boxCollider.Material));
			}

			PhysicsRigidbody.RecalculateAllInertia(bodies, boxes, spheres);

			bodies.Get(bodyId).ApplyImpulseAtPoint(_startImpulse, _startImpulsePoint);

			Debug.Log(bodies.Get(bodyId).LocalInertiaTensor);
		}

		private void Start()
		{
			Destroy(gameObject);
		}
	}
}