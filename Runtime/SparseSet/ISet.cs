namespace Massive
{
	public interface ISet : IReadOnlySet
	{
		int Ensure(int id);

		void Delete(int id);

		void DeleteDense(int dense);

		void SwapDense(int denseA, int denseB);
	}
}