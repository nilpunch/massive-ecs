using System.IO;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class CustomComponentParser<T> : IRegistryParser
	{
		private readonly IDataSetParser<T> _dataParser;

		public CustomComponentParser(IDataSetParser<T> dataParser)
		{
			_dataParser = dataParser;
		}

		public void Write(IRegistry registry, Stream stream)
		{
			var set = registry.Any<T>();

			SparseSetParser.Write(set, stream);

			if (set is IDataSet<T> dataSet)
			{
				_dataParser.Write(dataSet, stream);
			}
		}

		public void Read(IRegistry registry, Stream stream)
		{
			var set = registry.Any<T>();

			SparseSetParser.Read(set, stream);

			if (set is IDataSet<T> dataSet)
			{
				_dataParser.Read(dataSet, stream);
			}
		}
	}
}
