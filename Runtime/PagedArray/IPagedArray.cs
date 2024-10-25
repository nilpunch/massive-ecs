using System;

namespace Massive
{
	public interface IPagedArray
	{
		int PageSize { get; }
		Type ElementType { get; }

		Array GetPage(int page);
		void EnsurePage(int page);
	}
}
