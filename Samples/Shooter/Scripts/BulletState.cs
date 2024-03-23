using System.Collections.Generic;
using UnityEngine;

namespace Massive.Samples.Shooter
{
	public struct BulletState// : IManaged<BulletState>
	{
		public EntityTransform Transform;

		public Vector3 Velocity;

		public float Damage;

		public float Lifetime;

		public bool IsDestroyed => Lifetime <= 0f;
		
		public void Initialize()
		{
		}

		public void Reset()
		{
		}

		public void CopyTo(ref BulletState destination)
		{
			destination = this;
		}
	}
}