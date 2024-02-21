using UnityEngine;

namespace Massive.Samples.Physics
{
	public class MassiveBoxCollider : MonoBehaviour
	{
		[field: SerializeField] public Vector3 Size { get; private set; } = Vector3.one;
		[field: SerializeField] public PhysicMaterial Material { get; private set; } = new PhysicMaterial() { Density = 1f };

		private void OnDrawGizmos()
		{
			var orig = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawCube(Vector3.zero, Size);
			Gizmos.matrix = orig;
		}
	}
}