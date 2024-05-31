using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface ISetSelector
	{
		ISet[] Select(SetRegistry setRegistry);
	}

	public interface IReadOnlySetSelector
	{
		IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry);
	}

	public struct None : IOwnSelector, IIncludeSelector, IExcludeSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet[] Select(SetRegistry setRegistry) => Array.Empty<ISet>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry) => Array.Empty<IReadOnlySet>();
	}

	public struct Many<T> : IOwnSelector, IIncludeSelector, IExcludeSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet[] Select(SetRegistry setRegistry)
		{
			var result = new ISet[1];
			result[0] = setRegistry.Get<T>();
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			var result = new IReadOnlySet[1];
			result[0] = setRegistry.Get<T>();
			return result;
		}
	}

	public struct Many<T1, T2> : IOwnSelector, IIncludeSelector, IExcludeSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet[] Select(SetRegistry setRegistry)
		{
			var result = new ISet[2];
			result[0] = setRegistry.Get<T1>();
			result[1] = setRegistry.Get<T2>();
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			var result = new IReadOnlySet[2];
			result[0] = setRegistry.Get<T1>();
			result[1] = setRegistry.Get<T2>();
			return result;
		}
	}

	public struct Many<T1, T2, T3> : IOwnSelector, IIncludeSelector, IExcludeSelector
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			var result = new IReadOnlySet[3];
			result[0] = setRegistry.Get<T1>();
			result[1] = setRegistry.Get<T2>();
			result[2] = setRegistry.Get<T3>();
			return result;
		}
	}

	public struct Many<T1, T2, T3, TMany> : IOwnSelector, IIncludeSelector, IExcludeSelector
		where TMany : struct, IOwnSelector, IIncludeSelector, IExcludeSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet[] Select(SetRegistry setRegistry)
		{
			return default(TMany).Select(setRegistry).Concat(default(Many<T1, T2, T3>).Select(setRegistry)).ToArray();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry)
		{
			return default(TMany).SelectReadOnly(setRegistry).Concat(default(Many<T1, T2, T3>).SelectReadOnly(setRegistry)).ToArray();
		}
	}
}
