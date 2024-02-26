using System.Reflection;

namespace Massive.ECS
{
	/// <summary>
	/// Cross-platform component information.
	/// </summary>
	public static class ComponentMeta<T> where T : unmanaged
	{
		public static int SizeInBytes { get; }
		public static bool HasAnyFields { get; }

		static ComponentMeta()
		{
			unsafe
			{
				SizeInBytes = sizeof(T);
				HasAnyFields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Length > 0;
			}
		}
	}
}