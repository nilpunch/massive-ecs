using UnityEngine;

namespace Massive.Samples.Physics
{
	public static class ReferenceSpringing
	{
		public static void Apply(in MassiveData<PointMass> particles, in MassiveData<SoftBody> bodies, float deltaTime)
		{
			var particlesData = particles.Data;
			var aliveCount = particles.AliveCount;
			for (var i = 0; i < aliveCount; i++)
			{
				ref PointMass point = ref particlesData[i];
				SoftBody softBody = bodies.Get(point.SoftBodyId);

				Vector3 rotatedLocalPosition = (Vector3)(softBody.Rotation * point.LocalReferencePosition);
				
				Vector3 referencePoint = softBody.Centroid + rotatedLocalPosition;
				
				Vector3 displacement = referencePoint - point.Position;
				Vector3 movement = displacement * point.ReferenceSpring * deltaTime;

				point.AddMove(movement);
			}
		}
	}
}