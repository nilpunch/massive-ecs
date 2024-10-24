using System.IO;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class CustomComponentSerializer<T> : IRegistrySerializer
	{
		private readonly IDataSetSerializer _dataSerializer;

		public CustomComponentSerializer(IDataSetSerializer dataSerializer)
		{
			_dataSerializer = dataSerializer;
		}

		public void Serialize(Registry registry, Stream stream)
		{
			var set = registry.Set<T>();

			SerializationHelpers.WriteSparseSet(set, stream);

			if (set is DataSet<T> dataSet)
			{
				_dataSerializer.Write(dataSet, stream);
			}
		}

		public void Deserialize(Registry registry, Stream stream)
		{
			var set = registry.Set<T>();

			SerializationHelpers.ReadSparseSet(set, stream);

			if (set is DataSet<T> dataSet)
			{
				_dataSerializer.Read(dataSet, stream);
			}
		}
	}
}
