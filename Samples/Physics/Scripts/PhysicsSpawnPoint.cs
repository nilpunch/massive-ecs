using System;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public class PhysicsSpawnPoint : MonoBehaviour
	{
		[SerializeField] private float _boxSize;
		[SerializeField] private float _pointsMass = 1f;
		[SerializeField] private float _pointsDrag = 1f;
		[SerializeField] private float _referenceSpring = 1f;

		private void Start()
		{
			Destroy(gameObject);
		}

		public void Spawn(MassiveData<SoftBody> softBodies, MassiveData<PointMass> particles, MassiveData<Spring> springs)
		{
			// Calculate half extents based on box size
			Vector3 halfExtents = new Vector3(_boxSize, _boxSize, _boxSize) * 0.5f;

			// Calculate all world space corners using half extents and Transform.TransformPoint
			Vector3[] corners =
			{
				new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z),
				new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z),
				new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z),
				new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z),
				new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z),
				new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z),
				new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z),
				new Vector3(halfExtents.x, halfExtents.y, halfExtents.z),
			};
			
			Vector3[] transformedCorners =
			{
				transform.TransformPoint(corners[0]),
				transform.TransformPoint(corners[1]),
				transform.TransformPoint(corners[2]),
				transform.TransformPoint(corners[3]),
				transform.TransformPoint(corners[4]),
				transform.TransformPoint(corners[5]),
				transform.TransformPoint(corners[6]),
				transform.TransformPoint(corners[7]),
			};

			int softBodyId = softBodies.Create();

			particles.Create(new PointMass(softBodyId, transformedCorners[0], _pointsMass, _pointsDrag, false, corners[0], _referenceSpring));
			particles.Create(new PointMass(softBodyId, transformedCorners[1], _pointsMass, _pointsDrag, false, corners[1], _referenceSpring));
			particles.Create(new PointMass(softBodyId, transformedCorners[2], _pointsMass, _pointsDrag, false, corners[2], _referenceSpring));
			particles.Create(new PointMass(softBodyId, transformedCorners[3], _pointsMass, _pointsDrag, false, corners[3], _referenceSpring));
			particles.Create(new PointMass(softBodyId, transformedCorners[4], _pointsMass, _pointsDrag, false, corners[4], _referenceSpring));
			particles.Create(new PointMass(softBodyId, transformedCorners[5], _pointsMass, _pointsDrag, false, corners[5], _referenceSpring));
			particles.Create(new PointMass(softBodyId, transformedCorners[6], _pointsMass, _pointsDrag, false, corners[6], _referenceSpring));
			particles.Create(new PointMass(softBodyId, transformedCorners[7], _pointsMass, _pointsDrag, false, corners[7], _referenceSpring));
		}

		public void OnDrawGizmos()
		{
			// Calculate half extents based on box size
			Vector3 halfExtents = new Vector3(_boxSize, _boxSize, _boxSize) * 0.5f;

			// Calculate all world space corners using half extents and Transform.TransformPoint
			Vector3[] corners =
			{
				transform.TransformPoint(-halfExtents.x, -halfExtents.y, -halfExtents.z),
				transform.TransformPoint(halfExtents.x, -halfExtents.y, -halfExtents.z),
				transform.TransformPoint(-halfExtents.x, halfExtents.y, -halfExtents.z),
				transform.TransformPoint(halfExtents.x, halfExtents.y, -halfExtents.z),
				transform.TransformPoint(-halfExtents.x, -halfExtents.y, halfExtents.z),
				transform.TransformPoint(halfExtents.x, -halfExtents.y, halfExtents.z),
				transform.TransformPoint(-halfExtents.x, halfExtents.y, halfExtents.z),
				transform.TransformPoint(halfExtents.x, halfExtents.y, halfExtents.z),
			};

			// Wireframe
			Gizmos.DrawLine(corners[0], corners[1]);
			Gizmos.DrawLine(corners[1], corners[3]);
			Gizmos.DrawLine(corners[3], corners[2]);
			Gizmos.DrawLine(corners[2], corners[0]);
			Gizmos.DrawLine(corners[4], corners[5]);
			Gizmos.DrawLine(corners[5], corners[7]);
			Gizmos.DrawLine(corners[7], corners[6]);
			Gizmos.DrawLine(corners[6], corners[4]);
			Gizmos.DrawLine(corners[0], corners[4]);
			Gizmos.DrawLine(corners[1], corners[5]);
			Gizmos.DrawLine(corners[2], corners[6]);
			Gizmos.DrawLine(corners[3], corners[7]);
		}
	}
}