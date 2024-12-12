using System;

namespace Massive.Samples.Shooter
{
	public struct VelocityDamper
	{
		public float DampingFactor;

		public void DampVelocity(ref Velocity velocity, float deltaTime)
		{
			velocity.Value *= MathF.Exp(-DampingFactor * deltaTime);
		}
	}
}
