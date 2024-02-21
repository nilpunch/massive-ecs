using UnityEngine;

namespace Massive.Samples.Shooter
{
	public abstract class WorldUpdater : MonoBehaviour
	{
		public abstract void UpdateWorld(in WorldFrame worldFrame);
	}
}