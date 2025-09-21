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

		public static Query All<T>(this Query query) => query.All(query.World.Select<T>());

		public static Query All<T1, T2>(this Query query) => query.All(query.World.Select<T1, T2>());

		public static Query All<T1, T2, T3>(this Query query) => query.All(query.World.Select<T1, T2, T3>());

		public static Query All<T1, T2, T3, TAnd>(this Query query)
			where TAnd : IAndSelector, new() => query.All(query.World.Select<T1, T2, T3, TAnd>());

		public static Query None(this Query query, BitSet[] none)
		{
			query.Filter.SetNone(none);
			return query;
		}

		public static Query None<T>(this Query query) => query.None(query.World.Select<T>());

		public static Query None<T1, T2>(this Query query) => query.None(query.World.Select<T1, T2>());

		public static Query None<T1, T2, T3>(this Query query) => query.None(query.World.Select<T1, T2, T3>());

		public static Query None<T1, T2, T3, TAnd>(this Query query)
			where TAnd : IAndSelector, new() => query.None(query.World.Select<T1, T2, T3, TAnd>());
	}
}
