using System;

namespace Massive
{
	public interface IDataSet
	{
		Type ElementType { get; }

		Array GetPage(int page);

		void EnsurePage(int page);

		object GetRaw(int id);

		void SetRaw(int id, object value);
	}
}
