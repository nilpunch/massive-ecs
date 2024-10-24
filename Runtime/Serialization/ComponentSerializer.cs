using System.IO;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class ComponentSerializer<T> : IRegistrySerializer where T : unmanaged
	{
		public void Serialize(Registry registry, Stream stream)
		{
			var set = registry.Set<T>();

			SerializationHelpers.WriteSparseSet(set, stream);

			if (set is IDataSet dataSet)
			{
				SerializationHelpers.WriteUnmanagedPagedArray(dataSet.Data, set.Count, stream);
			}
		}

		public void Deserialize(Registry registry, Stream stream)
		{
			var set = registry.Set<T>();

			SerializationHelpers.ReadSparseSet(set, stream);

			if (set is IDataSet dataSet)
			{
				SerializationHelpers.ReadUnmanagedPagedArray(dataSet.Data, set.Count, stream);
			}
		}
	}
}
