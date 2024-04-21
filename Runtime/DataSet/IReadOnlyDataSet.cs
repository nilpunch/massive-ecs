using System;

namespace Massive
{
	public interface IReadOnlyDataSet<T> : IReadOnlySet where T : struct
	{
		Span<T> Data { get; }

		ref T Get(int id);
	}
}