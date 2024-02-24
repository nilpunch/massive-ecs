using UnityEngine;

namespace Massive.Samples.Physics
{
	public struct Transformation
	{
		public Vector3 Position;
		public Quaternion Rotation;

		public Transformation(Vector3 position, Quaternion rotation)
		{
			Position = position;
			Rotation = rotation;
		}

		public Transformation LocalToWorld(Transformation parent)
		{
			return new Transformation()
			{
				Position = parent.Position + parent.Rotation * Position,
				Rotation = parent.Rotation * Rotation,
			};
		}
	}
}