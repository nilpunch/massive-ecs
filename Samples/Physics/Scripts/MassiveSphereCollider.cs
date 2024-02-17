using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public class MassiveSphereCollider : MonoBehaviour
	{
		[field: SerializeField] public float Radius { get; private set; } = 1f;

		private void OnDrawGizmos()
		{
			Gizmos.DrawSphere(transform.position, Radius);
		}
	}
}