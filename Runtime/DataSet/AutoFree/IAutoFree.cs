using Massive.AutoFree;

namespace Massive
{
	/// <summary>
	/// Automatically frees allocator pointers when this component is removed from an entity.
	/// </summary>
	public interface IAutoFree<T> where T : unmanaged, IAutoFree<T>
	{
		[UnityEngine.Scripting.Preserve]
		public static SetAndCloner CreateDataSetAndCloner(Allocator allocator, object defaultValue)
		{
			var set = new AutoFreeDataSet<T>(allocator, (T)defaultValue);
			return new SetAndCloner(set, new DataSetCloner<T>(set));
		}
	}
}
