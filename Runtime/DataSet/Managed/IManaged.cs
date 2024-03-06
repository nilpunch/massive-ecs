namespace Massive
{
	public interface IManaged<T> where T : struct
	{
		void Initialize(out T data);
		void Reset(ref T data);
		void Clone(in T source, ref T destination);
	}

	public delegate void InitializeManaged<T>(out T data) where T : struct;
	public delegate void ResetManaged<T>(ref T data) where T : struct;
	public delegate void CloneManaged<T>(in T source, ref T destination) where T : struct;
}