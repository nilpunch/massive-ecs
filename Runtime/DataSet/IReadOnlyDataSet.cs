using System;

namespace Massive
{
	public interface IReadOnlyDataSet<T> : IReadOnlySet
	{
		Span<T> Data { get; }

		ref T Get(int id);
	}
}
