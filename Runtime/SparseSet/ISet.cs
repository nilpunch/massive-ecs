namespace Massive
{
	public interface ISet : IReadOnlySet
	{
		void Ensure(int id);

		void Delete(int id);

		void SwapDense(int denseA, int denseB);
	}
}