using Massive.Samples.ECS;
using UnityEngine;

namespace Massive.Samples.Shooter
{
	public abstract class WorldUpdater : MonoBehaviour
	{
		public abstract void Init(Registry registry);
		public abstract void UpdateWorld(float deltaTime);
	}
}