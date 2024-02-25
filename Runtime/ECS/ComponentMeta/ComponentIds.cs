using System.Collections.Generic;

namespace Massive.ECS
{
	/// <summary>
	/// Persistent ids used to avoid component ids collision.
	/// </summary>
	internal static class ComponentIds
	{
		internal static readonly HashSet<int> UsedIds = new HashSet<int>();
	}
}