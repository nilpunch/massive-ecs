using UnityEngine;

namespace Massive.Samples.Physics
{
	public class ParticleSpawnPoint : MonoBehaviour
	{
		public float Radius = 0.5f;
		public Vector3 Position => transform.position;

		private void Start()
		{
			Destroy(gameObject);
		}
	}
}