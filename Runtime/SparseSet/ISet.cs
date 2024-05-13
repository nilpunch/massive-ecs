namespace Massive
{
	public interface ISet : IReadOnlySet
	{
		void Assign(int id);

		void Unassign(int id);

		void Clear();

		void SwapDense(int denseA, int denseB);
	}
}
