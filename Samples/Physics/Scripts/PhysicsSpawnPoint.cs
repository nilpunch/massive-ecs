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

		public void Spawn(Massive<Rigidbody> softBodies, Massive<SphereCollider> spheres, Massive<BoxCollider> boxes)
		{
			int bodyId = softBodies.Create(new Rigidbody(transform.position, _mass, _restitution, isStatic: _static)
			{
				Velocity = _startVelocity,
				CenterOfMass = _centerOfMass,
				Rotation = transform.rotation,
			});

			if (_isBox)
			{
				boxes.Create(new BoxCollider(bodyId, _boxSize));
			}
			else
			{
				spheres.Create(new SphereCollider(bodyId, _radius));
			}
		}

		public void OnDrawGizmos()
		{
			if (_isBox)
			{
				var orig = Gizmos.matrix;
				Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.DrawCube(Vector3.zero, _boxSize);
				Gizmos.matrix = orig;
			}
			else
			{
				Gizmos.DrawSphere(transform.position, _radius);
			}
		}
	}
}