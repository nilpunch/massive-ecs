using System;

namespace Massive
{
	public interface IReadOnlyDataSet<T> : IReadOnlySet where T : unmanaged
	{
		Span<T> AliveData { get; }
		ref T Get(int id);
	}
}