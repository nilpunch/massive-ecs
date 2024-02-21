using UnityEngine;

namespace Massive.Samples.Physics
{
	public interface ISupportMappable
	{
		Vector3 Centre { get; }

		/// <summary>
		/// Returns furthest point of object in some direction.
		/// </summary>
		Vector3 SupportPoint(Vector3 direction);
	}
}