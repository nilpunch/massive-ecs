using System.Linq;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IExcludeSelector : IReadOnlySetSelector
	{
	}

	public struct Exclude<T> : IExcludeSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			return default(Many<T>).SelectReadOnly(setRegistry);
		}
	}

	public struct Exclude<T1, T2> : IExcludeSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			return default(Many<T1, T2>).SelectReadOnly(setRegistry);
		}
	}

	public struct Exclude<T1, T2, T3> : IExcludeSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			return default(Many<T1, T2, T3>).SelectReadOnly(setRegistry);
		}
	}

	public struct Exclude<T1, T2, T3, TExclude> : IExcludeSelector
		where TExclude : IExcludeSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			return default(TExclude).SelectReadOnly(setRegistry).Concat(default(Many<T1, T2, T3>).SelectReadOnly(setRegistry)).ToArray();
		}
	}
}
