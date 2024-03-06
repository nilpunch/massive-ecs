using UnityEngine;

namespace Massive.Samples.Physics
{
	public static class Gravity
	{
		public static void Apply(in DataSet<PhysicsRigidbody> rigidbodies, float gravity = 10f)
		{
			var aliveRigidbodies = rigidbodies.AliveData;
			for (var i = 0; i < aliveRigidbodies.Length; i++)
			{
				aliveRigidbodies[i].ApplyForce(Vector3.down * gravity);
			}
		}
	}
}