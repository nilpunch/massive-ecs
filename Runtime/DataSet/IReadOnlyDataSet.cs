using System;

namespace Massive
{
	public interface IReadOnlyDataSet<T> : IReadOnlySet
	{
		PagedArray<T> Data { get; }

		ref T Get(int id);
	}
}
