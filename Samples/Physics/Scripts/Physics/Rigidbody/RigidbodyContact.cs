using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public struct RigidbodyContact
	{
		public int BodyA;
		public int BodyB;

		public Vector3 ContactPointA;
		public Vector3 ContactPointB;
		public Vector3 Normal;
		public float Depth;
	}
}