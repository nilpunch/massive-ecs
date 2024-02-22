namespace Massive
{
	public interface ISparseSet : IReadOnlySet
	{
		CreateInfo Ensure(int id);
		CreateInfo Create();
		DeleteInfo? Delete(int id);
		DeleteInfo? DeleteDense(int dense);
	}
}