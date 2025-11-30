namespace Massive
{
	public interface ISystem
	{
		void Initialize(int id, Allocator allocator, World world);
	}
}
