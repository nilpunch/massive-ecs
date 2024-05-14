using System;

namespace Massive
{
	public interface IReadOnlyDataSet<T> : IReadOnlySet
	{
		PackedSpan<T> Data { get; }

		ref T Get(int id);
	}
}
