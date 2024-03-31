namespace Massive
{
	public interface ISet : IReadOnlySet
	{
		void Ensure(int id);

		void Remove(int id);

		void Clear();

		void SwapDense(int denseA, int denseB);
	}
}