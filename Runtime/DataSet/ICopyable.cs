namespace Massive
{
	public interface ICopyable<T>
	{
		void CopyTo(ref T other);
	}
}
