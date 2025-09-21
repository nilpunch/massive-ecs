using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class QueryExtensions
	{
		public static Query All(this Query query, BitSet[] all)
		{
			query.Filter.SetAll(all);
			return query;
		}

		public static Query All<T>(this Query query) => query.All(query.World.SelectSets<T>());

		public static Query All<T1, T2>(this Query query) => query.All(query.World.SelectSets<T1, T2>());

		public static Query All<T1, T2, T3>(this Query query) => query.All(query.World.SelectSets<T1, T2, T3>());

		public static Query All<T1, T2, T3, T4>(this Query query) => query.All(query.World.SelectSets<T1, T2, T3, T4>());

		public static Query All<T1, T2, T3, T4, T5>(this Query query) => query.All(query.World.SelectSets<T1, T2, T3, T4, T5>());

		public static Query All<T1, T2, T3, T4, T5, T6>(this Query query) => query.All(query.World.SelectSets<T1, T2, T3, T4, T5, T6>());

		public static Query All<T1, T2, T3, T4, T5, T6, TAnd>(this Query query)
			where TAnd : IAndSelector, new() => query.All(query.World.SelectSets<T1, T2, T3, T4, T5, T6, TAnd>());

		public static Query None(this Query query, BitSet[] none)
		{
			query.Filter.SetNone(none);
			return query;
		}

		public static Query None<T>(this Query query) => query.None(query.World.SelectSets<T>());

		public static Query None<T1, T2>(this Query query) => query.None(query.World.SelectSets<T1, T2>());

		public static Query None<T1, T2, T3>(this Query query) => query.None(query.World.SelectSets<T1, T2, T3>());

		public static Query None<T1, T2, T3, T4>(this Query query) => query.None(query.World.SelectSets<T1, T2, T3, T4>());

		public static Query None<T1, T2, T3, T4, T5>(this Query query) => query.None(query.World.SelectSets<T1, T2, T3, T4, T5>());

		public static Query None<T1, T2, T3, T4, T5, T6>(this Query query) => query.None(query.World.SelectSets<T1, T2, T3, T4, T5, T6>());

		public static Query None<T1, T2, T3, T4, T5, T6, TAnd>(this Query query)
			where TAnd : IAndSelector, new() => query.None(query.World.SelectSets<T1, T2, T3, T4, T5, T6, TAnd>());
	}
}
