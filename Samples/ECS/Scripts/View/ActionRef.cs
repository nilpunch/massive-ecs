namespace Massive.Samples.ECS
{
	public delegate void ActionRef<T>(ref T value);

	public delegate void ActionRef<T1, T2>(ref T1 value1, ref T2 value2);
}