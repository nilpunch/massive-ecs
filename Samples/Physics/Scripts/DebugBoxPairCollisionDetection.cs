using UnityEngine;

namespace Massive.Samples.Physics
{
	public class DebugBoxPairCollisionDetection : MonoBehaviour
	{
		[SerializeField] private Vector3 _firstSize;
		[SerializeField] private Vector3 _secondSize;
		[Space] [SerializeField] private UnityEngine.Transform _firstTransform;
		[SerializeField] private UnityEngine.Transform _secondTransform;
		[SerializeField, Range(0, 24)] private int _iterations = 0;

		private void OnDrawGizmos()
		{
			if (_firstTransform == null || _secondTransform == null)
				return;

			PhysicsBoxCollider firstBox = new PhysicsBoxCollider(0, _firstSize, new Transformation(), new PhysicMaterial())
			{
				World = new Transformation(_firstTransform.position, _firstTransform.rotation)
			};

			PhysicsBoxCollider secondBox = new PhysicsBoxCollider(0, _secondSize, new Transformation(), new PhysicMaterial())
			{
				World = new Transformation(_secondTransform.position, _secondTransform.rotation)
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
				var epaResult = EpaAlgorithm.Calculate(gjkResult.Simplex, firstBox, secondBox, _iterations);

				Gizmos.color = Color.red;
				Gizmos.DrawSphere(epaResult.ContactFirst.Position, 0.1f);
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(epaResult.ContactFirst.Position, epaResult.ContactFirst.Position + epaResult.PenetrationNormal * epaResult.PenetrationDepth);
			}
		}
	}
}
