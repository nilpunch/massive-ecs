using System.Linq;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IIncludeSelector : IReadOnlySetSelector
	{
	}

	public struct Include<T> : IIncludeSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			return default(Many<T>).SelectReadOnly(setRegistry);
		}
	}

	public struct Include<T1, T2> : IIncludeSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			return default(Many<T1, T2>).SelectReadOnly(setRegistry);
		}
	}

	public struct Include<T1, T2, T3> : IIncludeSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			return default(Many<T1, T2, T3>).SelectReadOnly(setRegistry);
		}
	}

	public struct Include<T1, T2, T3, TInclude> : IIncludeSelector
		where TInclude : IIncludeSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			return default(TInclude).SelectReadOnly(setRegistry).Concat(default(Many<T1, T2, T3>).SelectReadOnly(setRegistry)).ToArray();
		}
	}
}
