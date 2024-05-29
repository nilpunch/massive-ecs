using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IOwnSelector : ISetSelector
	{
	}

	public struct Own<T> : IOwnSelector
	{
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Select(SetRegistry setRegistry, ArraySegment<ISet> result)
		{
			new Many<T>().Select(setRegistry, result);
		}
	}

	public struct Own<T1, T2> : IOwnSelector
	{
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => 2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Select(SetRegistry setRegistry, ArraySegment<ISet> result)
		{
			new Many<T1, T2>().Select(setRegistry, result);
		}
	}

	public struct Own<T1, T2, T3> : IOwnSelector
	{
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => 3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Select(SetRegistry setRegistry, ArraySegment<ISet> result)
		{
			new Many<T1, T2, T3>().Select(setRegistry, result);
		}
	}
}
