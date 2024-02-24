using System;

namespace Massive
{
	public interface IDataSet<T> : ISet where T : struct
	{
		Span<T> AliveData { get; }
		CreateInfo Ensure(int id, T data);
		CreateInfo Create(T data);
		ref T Get(int id);
	}
}