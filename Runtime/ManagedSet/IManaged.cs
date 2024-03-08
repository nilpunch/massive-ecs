namespace Massive
{
	public partial interface IManaged<T> where T : struct, IManaged<T>
	{
		void Initialize();
		void Reset();
		void CopyTo(ref T destination);
	}
}