using System.Linq;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IIncludeSelector : IReadOnlySetSelector
	{
	}

	public class Include<T> : ReadOnlySelector<T>, IIncludeSelector
	{
	}

	public class Include<T1, T2> : ReadOnlySelector<T1, T2>, IIncludeSelector
	{
	}

	public class Include<T1, T2, T3> : ReadOnlySelector<T1, T2, T3>, IIncludeSelector
	{
	}

	public class Include<T1, T2, T3, TInclude> : ReadOnlySelector<T1, T2, T3, TInclude>, IIncludeSelector
		where TInclude : IIncludeSelector, new()
	{
	}
}
