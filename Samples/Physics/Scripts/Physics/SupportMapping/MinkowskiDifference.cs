using System;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public readonly struct MinkowskiDifference : IEquatable<MinkowskiDifference>
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

		public bool Equals(MinkowskiDifference other)
		{
			return Difference.Equals(other.Difference);
		}

		public override bool Equals(object obj)
		{
			return obj is MinkowskiDifference other && Equals(other);
		}

		public override int GetHashCode()
		{
			return Difference.GetHashCode();
		}
	}
}
