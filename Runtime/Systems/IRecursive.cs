namespace Massive
{
	public interface IRecursive<TSystemMethod> where TSystemMethod : ISystemMethodBase<TSystemMethod>
	{
		bool NeedRerun { get; }
	}
}
