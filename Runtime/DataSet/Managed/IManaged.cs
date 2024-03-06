namespace Massive
{
	public interface IManaged<T> where T : struct
	{
		void Initialize();
		void Reset();
		void CopyTo(ref T destination);
	}
}