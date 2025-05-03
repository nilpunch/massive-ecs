﻿using System.Runtime.CompilerServices;
using System.Threading;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	internal static class TypesCounter
	{
		private static int s_value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int Increment()
		{
			return Interlocked.Increment(ref s_value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int Get()
		{
			return s_value;
		}
	}
}
