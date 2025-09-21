using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class QueryExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query All(this Query query, BitSet[] all)
		{
			query.Filter.SetAll(all);
			return query;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query All<T>(this Query query)
		{
			return query.All(query.World.Select<T>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query All<T1, T2>(this Query query)
		{
			return query.All(query.World.Select<T1, T2>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query All<T1, T2, T3>(this Query query)
		{
			return query.All(query.World.Select<T1, T2, T3>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query All<T1, T2, T3, TAnd>(this Query query)
			where TAnd : IAndSelector, new()
		{
			return query.All(query.World.Select<T1, T2, T3, TAnd>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query None(this Query query, BitSet[] none)
		{
			query.Filter.SetNone(none);
			return query;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query None<T>(this Query query)
		{
			return query.None(query.World.Select<T>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query None<T1, T2>(this Query query)
		{
			return query.None(query.World.Select<T1, T2>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query None<T1, T2, T3>(this Query query)
		{
			return query.None(query.World.Select<T1, T2, T3>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query None<T1, T2, T3, TAnd>(this Query query)
			where TAnd : IAndSelector, new()
		{
			return query.None(query.World.Select<T1, T2, T3, TAnd>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Query Any(this Query query, BitSet[] any)
		{
			query.Filter.SetAny(any);
			return query;
		}
	}
}
