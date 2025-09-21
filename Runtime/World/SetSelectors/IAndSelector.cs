namespace Massive
{
	public interface IAndSelector : ISetSelector
	{
	}

	public class And<T> : Selector<T>, IAndSelector
	{
	}

	public class And<T1, T2> : Selector<T1, T2>, IAndSelector
	{
	}

	public class And<T1, T2, T3> : Selector<T1, T2, T3>, IAndSelector
	{
	}

	public class And<T1, T2, T3, TAnd> : Selector<T1, T2, T3, TAnd>, IAndSelector
		where TAnd : IAndSelector, new()
	{
	}
}
