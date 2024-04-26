using UnityEngine;

namespace Massive.Samples.Physics
{
	public class MassiveSphereCollider : MonoBehaviour
	{
		[field: SerializeField] public float Radius { get; private set; } = 1f;
		[field: SerializeField] public PhysicMaterial Material { get; private set; } = new PhysicMaterial() { Density = 1f };

		private void OnDrawGizmos()
		{
			Gizmos.DrawSphere(transform.position, Radius);
		}
	}
}
