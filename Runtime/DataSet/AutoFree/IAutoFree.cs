using Massive.AutoFree;

namespace Massive
{
	/// <summary>
	/// Binds component lifetime to the 
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
