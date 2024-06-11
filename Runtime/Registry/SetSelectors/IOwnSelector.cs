using System.Linq;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IOwnSelector : ISetSelector
	{
	}

	public class Own<T> : Selector<T>, IOwnSelector
	{
	}

	public class Own<T1, T2> : Selector<T1, T2>, IOwnSelector
	{
	}

	public class Own<T1, T2, T3> : Selector<T1, T2, T3>, IOwnSelector
	{
	}

	public class Own<T1, T2, T3, TOwn> : Selector<T1, T2, T3, TOwn>, IOwnSelector
		where TOwn : IOwnSelector, new()
	{
	}
}
