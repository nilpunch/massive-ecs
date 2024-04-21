namespace Massive
{
	public interface ISet : IReadOnlySet
	{
		int[] Dense { get; }

		int[] Sparse { get; }

		new int Count { get; set; }

		void Assign(int id);

		void Unassign(int id);

		void Clear();

		void SwapDense(int denseA, int denseB);

		void ResizeDense(int dataCapacity);

		void ResizeSparse(int capacity);
	}
}