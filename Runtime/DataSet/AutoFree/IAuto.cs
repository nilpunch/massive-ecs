using Massive.AutoFree;

namespace Massive
{
	/// <summary>
	/// Automatically clones nested allocator pointers and their associated data.<br/>
	/// Automatically frees nested pointers when this component is removed from an entity.
	/// </summary>
	public interface IAuto<T> where T : unmanaged, IAuto<T>
	{
		[UnityEngine.Scripting.Preserve]
		public static SetAndCloner CreateDataSetAndCloner(Allocator allocator, object defaultValue)
		{
			var set = new AutoDataSet<T>(allocator, (T)defaultValue);
			return new SetAndCloner(set, new DataSetCloner<T>(set));
		}
	}
}
