using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public class PhysicsSpawnPoint : MonoBehaviour
	{
		[SerializeField] private bool _isBox = false;
		[SerializeField] private Vector3 _boxSize = Vector3.one;
		[SerializeField] private float _radius = 1f;
		[SerializeField] private float _mass = 1f;
		[SerializeField] private float _restitution = 1f;
		[SerializeField] private bool _static = false;
		[SerializeField] private Vector3 _startVelocity = Vector3.zero;
		[SerializeField] private Vector3 _centerOfMass = Vector3.zero;

		private void Start()
		{
			Destroy(gameObject);
		}

		public void Spawn(Massive<Rigidbody> softBodies, Massive<SphereCollider> colliders)
		{
			int bodyId = softBodies.Create(new Rigidbody(transform.position, _mass, _restitution, isStatic: _static) { Velocity = _startVelocity, CenterOfMass = _centerOfMass });

			colliders.Create(new SphereCollider(bodyId, _radius));
		}

		public void OnDrawGizmos()
		{
			if (_isBox)
			{
				Gizmos.DrawCube(transform.position, _boxSize);
			}
			else
			{
				Gizmos.DrawSphere(transform.position, _radius);
			}
		}
	}
}