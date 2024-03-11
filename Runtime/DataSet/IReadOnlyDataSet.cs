using System;
using System.Diagnostics.Contracts;

namespace Massive
{
	public interface IReadOnlyDataSet<T> : IReadOnlySet where T : struct
	{
		[Pure]
		Span<T> AliveData { get; }

		[Pure]
		ref T Get(int id);
	}
}