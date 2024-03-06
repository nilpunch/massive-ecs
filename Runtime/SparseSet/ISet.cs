namespace Massive
{
	public interface ISet : IReadOnlySet
	{
		CreateInfo Ensure(int id);
		DeleteInfo? Delete(int id);
		DeleteInfo? DeleteDense(int dense);
		void SwapDense(int denseA, int denseB);
	}
}