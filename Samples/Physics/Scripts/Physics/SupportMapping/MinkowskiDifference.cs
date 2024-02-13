using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public readonly struct MinkowskiDifference
	{
		public readonly Vector3 SupportA;
		public readonly Vector3 SupportB;
		public readonly Vector3 Difference;

		private MinkowskiDifference(Vector3 supportA, Vector3 supportB, Vector3 difference)
		{
			SupportA = supportA;
			SupportB = supportB;
			Difference = difference;
		}

		public static MinkowskiDifference Calculate<T>(T shapeA, T shapeB, Vector3 direction) where T : ISupportMappable
		{
			Vector3 supportA = shapeA.SupportPoint(direction);
			Vector3 supportB = shapeB.SupportPoint(-direction);
			Vector3 difference = supportA - supportB;

			return new MinkowskiDifference(supportA, supportB, difference);
		}
	}
}