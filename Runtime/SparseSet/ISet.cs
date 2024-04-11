namespace Massive
{
	public interface ISet : IReadOnlySet
	{
		int[] Dense { get; }

		int[] Sparse { get; }

		new int AliveCount { get; set; }

		void Ensure(int id);

		void Remove(int id);

		void Clear();

		void SwapDense(int denseA, int denseB);

		void ResizeDense(int dataCapacity);

		void ResizeSparse(int capacity);
	}
}