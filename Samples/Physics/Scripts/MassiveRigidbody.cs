using UnityEngine;

namespace Massive.Samples.Physics
{
	public class MassiveRigidbody : MonoBehaviour
	{
		[SerializeField] private bool _static = false;
		[SerializeField] private Vector3 _startImpulse;
		[SerializeField] private Vector3 _startImpulsePoint;

		public void Spawn(Registry registry)
		{
			var bodyId = registry.Create(new PhysicsRigidbody()
			{
				WorldCenterOfMass = new Transformation(transform.position, transform.rotation),
				IsStatic = _static
			});

			foreach (var sphereCollider in GetComponentsInChildren<MassiveSphereCollider>())
			{
				registry.Create(new PhysicsSphereCollider(bodyId, sphereCollider.Radius,
					new Transformation(transform.InverseTransformPoint(sphereCollider.transform.position),
						Quaternion.Inverse(transform.rotation) * sphereCollider.transform.rotation),
					sphereCollider.Material));
			}

			foreach (var boxCollider in GetComponentsInChildren<MassiveBoxCollider>())
			{
				registry.Create(new PhysicsBoxCollider(bodyId, boxCollider.Size,
					new Transformation(transform.InverseTransformPoint(boxCollider.transform.position), Quaternion.Inverse(transform.rotation) * boxCollider.transform.rotation),
					boxCollider.Material));
			}

			PhysicsRigidbody.RecalculateAllInertia(registry.Components<PhysicsRigidbody>(), registry.Components<PhysicsBoxCollider>(), registry.Components<PhysicsSphereCollider>());

			registry.Get<PhysicsRigidbody>(bodyId).ApplyImpulseAtPoint(_startImpulse, _startImpulsePoint);
		}

		private void Start()
		{
			Destroy(gameObject);
		}
	}
}