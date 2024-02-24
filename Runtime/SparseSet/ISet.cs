namespace Massive
{
	public interface ISet : IReadOnlySet
	{
		CreateInfo Ensure(int id);
		CreateInfo Create();
		DeleteInfo? Delete(int id);
		DeleteInfo? DeleteDense(int dense);
	}
}