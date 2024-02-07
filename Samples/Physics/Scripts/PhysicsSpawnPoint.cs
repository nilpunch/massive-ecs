using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public class PhysicsSpawnPoint : MonoBehaviour
	{
		[SerializeField] private float _radius = 1f;
		[SerializeField] private float _mass = 1f;
		[SerializeField] private float _restitution = 1f;
		[SerializeField] private bool _static = false;
		[SerializeField] private Vector3 _startVelocity = Vector3.zero;

		private void Start()
		{
			Destroy(gameObject);
		}

		public void Spawn(Massive<Rigidbody> softBodies, Massive<SphereCollider> colliders)
		{
			int bodyId = softBodies.Create(new Rigidbody(transform.position, _mass, _restitution, isStatic: _static) { Velocity = _startVelocity });
			colliders.Create(new SphereCollider(bodyId, _radius));
		}

		public void OnDrawGizmos()
		{
			Gizmos.DrawSphere(transform.position, _radius);
		}
	}
}