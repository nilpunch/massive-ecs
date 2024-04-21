using UnityEngine;

namespace Massive.Samples.Physics
{
	public static class Gravity
	{
		public static void Apply(in IReadOnlyDataSet<PhysicsRigidbody> rigidbodies, float gravity = 10f)
		{
			var aliveRigidbodies = rigidbodies.Data;
			for (var i = 0; i < aliveRigidbodies.Length; i++)
			{
				aliveRigidbodies[i].ApplyForce(Vector3.down * gravity);
			}
		}
	}
}