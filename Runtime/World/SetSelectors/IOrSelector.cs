namespace Massive
{
	public interface IOrSelector : ISetSelector
	{
	}

	public class Or<T> : Selector<T>, IOrSelector
	{
	}

	public class Or<T1, T2> : Selector<T1, T2>, IOrSelector
	{
	}

	public class Or<T1, T2, T3> : Selector<T1, T2, T3>, IOrSelector
	{
	}

	public class Or<T1, T2, T3, TOr> : Selector<T1, T2, T3, TOr>, IOrSelector
		where TOr : IOrSelector, new()
	{
	}
}
