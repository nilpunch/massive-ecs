using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public struct RigidbodyContact
	{
		public int BodyA;
		public int BodyB;

		public Vector3 ContactPoint;
		public Vector3 Normal;
		public float Depth;
	}
}