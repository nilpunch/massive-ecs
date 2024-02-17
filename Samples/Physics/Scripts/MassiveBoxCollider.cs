using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public class MassiveBoxCollider : MonoBehaviour
	{
		[field: SerializeField] public Vector3 Size { get; private set; } = Vector3.one;

		private void OnDrawGizmos()
		{
			var orig = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawCube(Vector3.zero, Size);
			Gizmos.matrix = orig;
		}
	}
}