using UnityEngine;

namespace Massive.Samples.Physics
{
	public class PhysicsSpawnPoint : MonoBehaviour
	{
		[SerializeField] private float _boxSize;
		[SerializeField] private float _particlesRadius;
		[SerializeField] private float _strength = 100f;
		[SerializeField] private float _elongation = 1f;
		
		private void Start()
		{
			Destroy(gameObject);
		}
		
		public void Spawn(in Frame<Particle> particles, in Frame<Spring> springs)
		{
			// Calculate half extents based on box size
			Vector3 halfExtents = new Vector3(_boxSize, _boxSize, _boxSize) * 0.5f;

			// Calculate all world space corners using half extents and Transform.TransformPoint
			Vector3[] corners = {
				transform.TransformPoint(-halfExtents.x, -halfExtents.y, -halfExtents.z),
				transform.TransformPoint(halfExtents.x, -halfExtents.y, -halfExtents.z),
				transform.TransformPoint(-halfExtents.x, halfExtents.y, -halfExtents.z),
				transform.TransformPoint(halfExtents.x, halfExtents.y, -halfExtents.z),
				transform.TransformPoint(-halfExtents.x, -halfExtents.y, halfExtents.z),
				transform.TransformPoint(halfExtents.x, -halfExtents.y, halfExtents.z),
				transform.TransformPoint(-halfExtents.x, halfExtents.y, halfExtents.z),
				transform.TransformPoint(halfExtents.x, halfExtents.y, halfExtents.z),
			};

			int[] particlesIds = {
				particles.Create(new Particle(corners[0], _particlesRadius)),
				particles.Create(new Particle(corners[1], _particlesRadius)),
				particles.Create(new Particle(corners[2], _particlesRadius)),
				particles.Create(new Particle(corners[3], _particlesRadius)),
				particles.Create(new Particle(corners[4], _particlesRadius)),
				particles.Create(new Particle(corners[5], _particlesRadius)),
				particles.Create(new Particle(corners[6], _particlesRadius)),
				particles.Create(new Particle(corners[7], _particlesRadius)),
			};

			for (int i = 0; i < 8; i++)
			{
				for (int j = i + 1; j < 8; j++)
				{
					// Create springs between all pairs of particles (corners)
					springs.Create(new Spring(particlesIds[i], particlesIds[j], Vector3.Distance(corners[i], corners[j]), _strength, _elongation));
				}
			}
		}
	}
}