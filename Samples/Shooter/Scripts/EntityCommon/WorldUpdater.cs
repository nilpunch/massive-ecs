using UnityEngine;

namespace Massive.Samples.Shooter
{
	public abstract class WorldUpdater : MonoBehaviour
	{
		public abstract void Init(IRegistry registry);
		public abstract void UpdateWorld(float deltaTime);
	}
}