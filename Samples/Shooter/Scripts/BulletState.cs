using UnityEngine;

namespace Massive.Samples.Shooter
{
	public struct BulletState : IManaged<BulletState>
	{
		public EntityTransform Transform;

		public Vector3 Velocity;

		public float Damage;

		public float Lifetime;

		public bool IsDestroyed => Lifetime <= 0f;
		
		public void Initialize(out BulletState data)
		{
			data = default;
		}

		public void Reset(ref BulletState data)
		{
			data = default;
		}

		public void Clone(in BulletState source, ref BulletState destination)
		{
			destination = source;
		}
	}
}