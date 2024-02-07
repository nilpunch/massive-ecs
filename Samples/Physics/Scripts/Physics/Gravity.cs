using System;
using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public static class Gravity
	{
		public static void Apply(in Massive<Rigidbody> rigidbodies, float gravity = 10f)
		{
			var aliveRigidbodies = rigidbodies.AliveData;
			for (var i = 0; i < aliveRigidbodies.Length; i++)
			{
				aliveRigidbodies[i].ApplyForce(Vector3.down * gravity);
			}
		}
	}
}