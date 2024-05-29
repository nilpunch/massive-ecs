using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface ISetSelector
	{
		int Count { get; }

		void Select(SetRegistry setRegistry, ArraySegment<ISet> result);
	}

	public interface IReadOnlySetSelector
	{
		int Count { get; }

		void Select(SetRegistry setRegistry, ArraySegment<IReadOnlySet> result);
	}

	public struct None : IOwnSelector, IIncludeSelector, IExcludeSelector
	{
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Select(SetRegistry setRegistry, ArraySegment<ISet> result)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Select(SetRegistry setRegistry, ArraySegment<IReadOnlySet> result)
		{
		}
	}

	public struct Many<T> : IOwnSelector, IIncludeSelector, IExcludeSelector
	{
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Select(SetRegistry setRegistry, ArraySegment<ISet> result)
		{
			result[0] = setRegistry.Get<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Select(SetRegistry setRegistry, ArraySegment<IReadOnlySet> result)
		{
			result[0] = setRegistry.Get<T>();
		}
	}

	public struct Many<T1, T2> : IOwnSelector, IIncludeSelector, IExcludeSelector
	{
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => 2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Select(SetRegistry setRegistry, ArraySegment<ISet> result)
		{
			result[0] = setRegistry.Get<T1>();
			result[1] = setRegistry.Get<T2>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Select(SetRegistry setRegistry, ArraySegment<IReadOnlySet> result)
		{
			result[0] = setRegistry.Get<T1>();
			result[1] = setRegistry.Get<T2>();
		}
	}

	public struct Many<T1, T2, T3> : IOwnSelector, IIncludeSelector, IExcludeSelector
	{
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => 3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Select(SetRegistry setRegistry, ArraySegment<ISet> result)
		{
			result[0] = setRegistry.Get<T1>();
			result[1] = setRegistry.Get<T2>();
			result[2] = setRegistry.Get<T3>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Select(SetRegistry setRegistry, ArraySegment<IReadOnlySet> result)
		{
			result[0] = setRegistry.Get<T1>();
			result[1] = setRegistry.Get<T2>();
			result[2] = setRegistry.Get<T3>();
		}
	}
}
