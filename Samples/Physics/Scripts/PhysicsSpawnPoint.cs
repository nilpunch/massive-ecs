using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public class PhysicsSpawnPoint : MonoBehaviour
	{
		[SerializeField] private float _radius = 1f;
		[SerializeField] private float _mass = 1f;
		[SerializeField] private bool _static = false;

		private void Start()
		{
			Destroy(gameObject);
		}

		public void Spawn(Massive<Rigidbody> softBodies, Massive<SphereCollider> colliders)
		{
			int bodyId = softBodies.Create(new Rigidbody(transform.position, _mass));
			colliders.Create(new SphereCollider(bodyId, _radius));
		}

		public void OnDrawGizmos()
		{
			Gizmos.DrawSphere(transform.position, _radius);
		}
	}
}