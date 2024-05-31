using System.Linq;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IOwnSelector : ISetSelector
	{
	}

	public struct Own<T> : IOwnSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet[] Select(SetRegistry setRegistry)
		{
			return default(Many<T>).Select(setRegistry);
		}
	}

	public struct Own<T1, T2> : IOwnSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet[] Select(SetRegistry setRegistry)
		{
			return default(Many<T1, T2>).Select(setRegistry);
		}
	}

	public struct Own<T1, T2, T3> : IOwnSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet[] Select(SetRegistry setRegistry)
		{
			return default(Many<T1, T2, T3>).Select(setRegistry);
		}
	}

	public struct Own<T1, T2, T3, TOwn> : IOwnSelector
		where TOwn : IOwnSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet[] Select(SetRegistry setRegistry)
		{
			return default(TOwn).Select(setRegistry).Concat(default(Many<T1, T2, T3>).Select(setRegistry)).ToArray();
		}
	}
}
