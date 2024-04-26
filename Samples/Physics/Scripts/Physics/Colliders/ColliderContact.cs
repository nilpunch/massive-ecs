using UnityEngine;

namespace Massive.Samples.Physics
{
	public struct ColliderContact
	{
		public Vector3 OffsetFromColliderA;
		public Vector3 Normal;
		public float Depth;
	}
}
