using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldQueryExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query All<T>(this World world)
		{
			return new Query(world).All<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query All<T1, T2>(this World world)
		{
			return new Query(world).All<T1, T2>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query All<T1, T2, T3>(this World world)
		{
			return new Query(world).All<T1, T2, T3>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query All<T1, T2, T3, TAnd>(this World world)
			where TAnd : IAndSelector, new()
		{
			return new Query(world).All<T1, T2, T3, TAnd>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query None<T>(this World world)
		{
			return new Query(world).None<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query None<T1, T2>(this World world)
		{
			return new Query(world).None<T1, T2>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query None<T1, T2, T3>(this World world)
		{
			return new Query(world).None<T1, T2, T3>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query None<T1, T2, T3, TAnd>(this World world)
			where TAnd : IAndSelector, new()
		{
			return new Query(world).None<T1, T2, T3, TAnd>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query Filter(this World world, Filter filter)
		{
			return new Query(world, filter);
		}
	}
}
