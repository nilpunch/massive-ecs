using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IIncludeSelector : IReadOnlySetSelector
	{
	}

	public struct Include<T> : IIncludeSelector
	{
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Select(SetRegistry setRegistry, ArraySegment<IReadOnlySet> result)
		{
			new Many<T>().Select(setRegistry, result);
		}
	}

	public struct Include<T1, T2> : IIncludeSelector
	{
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => 2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Select(SetRegistry setRegistry, ArraySegment<IReadOnlySet> result)
		{
			new Many<T1, T2>().Select(setRegistry, result);
		}
	}

	public struct Include<T1, T2, T3> : IIncludeSelector
	{
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => 3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Select(SetRegistry setRegistry, ArraySegment<IReadOnlySet> result)
		{
			new Many<T1, T2, T3>().Select(setRegistry, result);
		}
	}
}
