using System.Linq;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IExcludeSelector : IReadOnlySetSelector
	{
	}

	public class Exclude<T> : ReadOnlySelector<T>, IExcludeSelector
	{
	}

	public class Exclude<T1, T2> : ReadOnlySelector<T1, T2>, IExcludeSelector
	{
	}

	public class Exclude<T1, T2, T3> : ReadOnlySelector<T1, T2, T3>, IExcludeSelector
	{
	}

	public class Exclude<T1, T2, T3, TExclude> : ReadOnlySelector<T1, T2, T3, TExclude>, IExcludeSelector
		where TExclude : IExcludeSelector, new()
	{
	}
}
