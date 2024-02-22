using System;

namespace Massive
{
	public interface IDataSet<T> : IReadOnlySet where T : struct
	{
		Span<T> AliveData { get; }
		void Ensure(int id, T data = default);
		int Create(T data = default);
		void Delete(int id);
		void DeleteDense(int denseIndex);
	}
}