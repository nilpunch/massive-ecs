using System.Linq;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface ISetSelector
	{
		ISet[] Select(SetRegistry setRegistry);
	}

	public class Selector<T> : ISetSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet[] Select(SetRegistry setRegistry)
		{
			var result = new ISet[1];
			result[0] = setRegistry.Get<T>();
			return result;
		}
	}

	public class Selector<T1, T2> : ISetSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet[] Select(SetRegistry setRegistry)
		{
			var result = new ISet[2];
			result[0] = setRegistry.Get<T1>();
			result[1] = setRegistry.Get<T2>();
			return result;
		}
	}

	public class Selector<T1, T2, T3> : ISetSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet[] Select(SetRegistry setRegistry)
		{
			var result = new ISet[3];
			result[0] = setRegistry.Get<T1>();
			result[1] = setRegistry.Get<T2>();
			result[2] = setRegistry.Get<T3>();
			return result;
		}
	}

	public class Selector<T1, T2, T3, TSelector> : ISetSelector
		where TSelector : ISetSelector, new()
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet[] Select(SetRegistry setRegistry)
		{
			return new TSelector().Select(setRegistry).Concat(new Selector<T1, T2, T3>().Select(setRegistry)).ToArray();
		}
	}
}
