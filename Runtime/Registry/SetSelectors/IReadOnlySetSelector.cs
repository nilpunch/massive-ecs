using System.Linq;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IReadOnlySetSelector
	{
		IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry);
	}

	public class ReadOnlySelector<T> : IReadOnlySetSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			var result = new IReadOnlySet[1];
			result[0] = setRegistry.Get<T>();
			return result.Distinct().ToArray();
		}
	}

	public class ReadOnlySelector<T1, T2> : IReadOnlySetSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			var result = new IReadOnlySet[2];
			result[0] = setRegistry.Get<T1>();
			result[1] = setRegistry.Get<T2>();
			return result.Distinct().ToArray();
		}
	}

	public class ReadOnlySelector<T1, T2, T3> : IReadOnlySetSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			var result = new IReadOnlySet[3];
			result[0] = setRegistry.Get<T1>();
			result[1] = setRegistry.Get<T2>();
			result[2] = setRegistry.Get<T3>();
			return result.Distinct().ToArray();
		}
	}

	public class ReadOnlySelector<T1, T2, T3, TSelector> : IReadOnlySetSelector
		where TSelector : IReadOnlySetSelector, new()
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			return new TSelector().SelectReadOnly(setRegistry).Concat(new ReadOnlySelector<T1, T2, T3>().SelectReadOnly(setRegistry)).Distinct().ToArray();
		}
	}
}
