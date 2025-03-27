namespace Massive
{
	public interface IIncludeSelector : ISetSelector
	{
	}

	public class Include<T> : Selector<T>, IIncludeSelector
	{
	}

	public class Include<T1, T2> : Selector<T1, T2>, IIncludeSelector
	{
	}

	public class Include<T1, T2, T3> : Selector<T1, T2, T3>, IIncludeSelector
	{
	}

	public class Include<T1, T2, T3, TInclude> : Selector<T1, T2, T3, TInclude>, IIncludeSelector
		where TInclude : IIncludeSelector, new()
	{
	}
}
