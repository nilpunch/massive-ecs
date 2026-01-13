using System;

namespace Massive
{
	public interface IDataSet
	{
		BitSet BitSet { get; }

		Type ElementType { get; }

		Type ArrayType { get; }

		Array GetPage(int page);

		void EnsurePage(int page);

		object GetRaw(int id);

		void SetRaw(int id, object value);

		DataPageEnumerable GetDataPages();
	}
}
