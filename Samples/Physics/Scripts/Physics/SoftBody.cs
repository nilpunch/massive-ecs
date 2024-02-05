using UnityEngine;

namespace Massive.Samples.Physics
{
	public struct SoftBody
	{
		public int PointsCount;
		public Vector3 Center;
		public Quaternion Rotation;
		
		private Vector4 RawRotation;

		public SoftBody(int pointsCount)
		{
			PointsCount = pointsCount;
			Rotation = Quaternion.identity;
			Center = Vector3.zero;
			RawRotation = Vector4.zero;
		}
		
		public static void UpdateAll(MassiveData<SoftBody> softBodies, MassiveData<PointMass> points)
		{
			var pointsData = points.Data;
			var softBodiesData = softBodies.Data;

			// Reset average
			int aliveBodies = softBodies.AliveCount;
			for (int i = 0; i < aliveBodies; i++)
			{
				softBodiesData[i].Center = Vector3.zero;
				softBodiesData[i].RawRotation = Vector3.zero;
			}

			// Calculate center
			int alivePoints = points.AliveCount;
			for (int i = 0; i < alivePoints; i++)
			{
				var pointMass = pointsData[i];
				softBodies.Get(pointMass.SoftBodyId).Center += pointMass.Position;
			}
			for (int i = 0; i < aliveBodies; i++)
			{
				softBodiesData[i].Center /= softBodiesData[i].PointsCount;
			}
			
			// Calculate rotation
			for (int i = 0; i < alivePoints; i++)
			{
				var pointMass = pointsData[i];
				ref var softBody = ref softBodies.Get(pointMass.SoftBodyId);
				
				Quaternion rotation = Quaternion.FromToRotation(pointMass.Position - softBody.Center, pointMass.LocalReferencePosition);
				
				if (Quaternion.Dot(rotation, Quaternion.identity) < 0f)
				{
					softBody.RawRotation -= new Vector4(rotation.x, rotation.y, rotation.z, rotation.w);
				}
				else
				{
					softBody.RawRotation += new Vector4(rotation.x, rotation.y, rotation.z, rotation.w);
				}
			}
			for (int i = 0; i < aliveBodies; i++)
			{
				ref var softBody = ref softBodiesData[i];
				Vector4 rawRotation = softBody.RawRotation / softBody.PointsCount;
				softBody.Rotation = new Quaternion(rawRotation.x, rawRotation.y, rawRotation.z, rawRotation.w);
			}
		}
	}
}