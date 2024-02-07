using UnityEngine;

namespace MassiveData.Samples.Shooter
{
	public abstract class WorldUpdater : MonoBehaviour
	{
		public abstract void UpdateWorld(in WorldFrame worldFrame);
	}
}