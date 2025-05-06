namespace Massive
{
	public interface IAutoAlloc<T> where T : unmanaged, IAutoAlloc<T>
	{
		[UnityEngine.Scripting.Preserve]
		private static void ReflectionSupportForAOT()
		{
			_ = new AutoAllocatingDataSet<T>(default);
		}
	}
}
