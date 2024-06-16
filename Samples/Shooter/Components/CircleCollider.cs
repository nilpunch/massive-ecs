using System.Numerics;

namespace Massive.Samples.Shooter
{
	public struct CircleCollider
	{
		public float Radius;

		public static bool IsCollided(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB)
		{
			return radiusA + radiusB - Vector2.Distance(centerA, centerB) >= 0f;
		}
	}
}
