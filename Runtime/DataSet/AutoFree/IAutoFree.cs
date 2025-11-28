using Massive.AutoFree;

namespace Massive
{
	/// <summary>
	/// Automatically frees allocator pointers when this component is removed from an entity.
	/// </summary>
	public interface IAutoFree<T> where T : unmanaged, IAutoFree<T>
	{
		[UnityEngine.Scripting.Preserve]
		private static void ReflectionSupportForAOT()
		{
			_ = new AutoFreeDataSet<T>(null);
		}
	}
}
