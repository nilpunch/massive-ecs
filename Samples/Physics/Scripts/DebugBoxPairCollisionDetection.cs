using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public class DebugBoxPairCollisionDetection : MonoBehaviour
	{
		[SerializeField] private Vector3 _firstSize;
		[SerializeField] private Vector3 _secondSize;
		[Space]
		[SerializeField] private Transform _firstTransform;
		[SerializeField] private Transform _secondTransform;

		private void OnDrawGizmos()
		{
			if (_firstTransform == null || _secondTransform == null)
				return;

			BoxCollider firstBox = new BoxCollider(0, _firstSize, Vector3.zero, Quaternion.identity)
			{
				WorldPosition = _firstTransform.position,
				WorldRotation = _firstTransform.rotation,
			};

			BoxCollider secondBox = new BoxCollider(0, _secondSize, Vector3.zero, Quaternion.identity)
			{
				WorldPosition = _secondTransform.position,
				WorldRotation = _secondTransform.rotation,
			};

			var orig = Gizmos.matrix;
			Gizmos.matrix = _firstTransform.localToWorldMatrix;
			Gizmos.DrawCube(Vector3.zero, _firstSize);
			Gizmos.matrix = _secondTransform.localToWorldMatrix;
			Gizmos.DrawCube(Vector3.zero, _secondSize);
			Gizmos.matrix = orig;

			var gjkResult = GjkAlgorithm.Calculate(firstBox, secondBox);

			if (gjkResult.CollisionHappened)
			{
				var epaResult = EpaAlgorithm.Calculate(gjkResult.Simplex, firstBox, secondBox);

				Gizmos.color = Color.red;
				Gizmos.DrawSphere((epaResult.ContactFirst.Position + epaResult.ContactSecond.Position) / 2f, 0.1f);
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine((epaResult.ContactFirst.Position + epaResult.ContactSecond.Position) / 2f,
					(epaResult.ContactFirst.Position + epaResult.ContactSecond.Position) / 2f + epaResult.PenetrationNormal * epaResult.PenetrationDepth);
			}
		}
	}
}