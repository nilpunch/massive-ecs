namespace Massive
{
	public interface IManaged<T> where T : struct
	{
		void Initialize(out T data);
		void Reset(ref T data);
		void Clone(in T source, ref T destination);
	}
}