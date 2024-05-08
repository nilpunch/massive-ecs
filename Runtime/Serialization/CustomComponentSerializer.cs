using System.IO;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class CustomComponentSerializer<T> : IRegistrySerializer
	{
		private readonly IDataSetSerializer<T> _dataSerializer;

		public CustomComponentSerializer(IDataSetSerializer<T> dataSerializer)
		{
			_dataSerializer = dataSerializer;
		}

		public void Serialize(IRegistry registry, Stream stream)
		{
			var set = (SparseSet)registry.Any<T>();

			SparseSetSerializer.Serialize(set, stream);

			if (set is DataSet<T> dataSet)
			{
				_dataSerializer.Write(dataSet, stream);
			}
		}

		public void Deserialize(IRegistry registry, Stream stream)
		{
			var set = (SparseSet)registry.Any<T>();

			SparseSetSerializer.Deserialize(set, stream);

			if (set is DataSet<T> dataSet)
			{
				_dataSerializer.Read(dataSet, stream);
			}
		}
	}
}
