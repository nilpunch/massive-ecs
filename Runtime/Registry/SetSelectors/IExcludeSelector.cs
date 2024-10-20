namespace Massive
{
	public interface IExcludeSelector : ISetSelector
	{
	}

	public class Exclude<T> : Selector<T>, IExcludeSelector
	{
	}

	public class Exclude<T1, T2> : Selector<T1, T2>, IExcludeSelector
	{
	}

	public class Exclude<T1, T2, T3> : Selector<T1, T2, T3>, IExcludeSelector
	{
	}

	public class Exclude<T1, T2, T3, TExclude> : Selector<T1, T2, T3, TExclude>, IExcludeSelector
		where TExclude : IExcludeSelector, new()
	{
	}
}
