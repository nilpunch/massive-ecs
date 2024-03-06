namespace Massive
{
	public class DefaultManagedCloner<T> : IManagedCloner<T> where T : struct
	{
		public void Initialize(out T data)
		{
			data = default;
		}

		public void Reset(ref T data)
		{
			data = default;
		}

		public void Clone(in T source, ref T destination)
		{
			destination = source;
		}
	}
}