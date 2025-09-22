using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldQueryExtensions
	{
		public static Query Include<T>(this World world) => new Query(world).Include<T>();

		public static Query Include<T1, T2>(this World world) => new Query(world).Include<T1, T2>();

		public static Query Include<T1, T2, T3>(this World world) => new Query(world).Include<T1, T2, T3>();

		public static Query Include<T1, T2, T3, T4>(this World world) => new Query(world).Include<T1, T2, T3, T4>();

		public static Query Include<T1, T2, T3, T4, T5>(this World world) => new Query(world).Include<T1, T2, T3, T4, T5>();

		public static Query Include<T1, T2, T3, T4, T5, T6>(this World world) => new Query(world).Include<T1, T2, T3, T4, T5, T6>();

		public static Query Include<T1, T2, T3, T4, T5, T6, TAnd>(this World world)
			where TAnd : IAndSelector, new() => new Query(world).Include<T1, T2, T3, T4, T5, T6, TAnd>();

		public static Query Exclude<T>(this World world) => new Query(world).Exclude<T>();

		public static Query Exclude<T1, T2>(this World world) => new Query(world).Exclude<T1, T2>();

		public static Query Exclude<T1, T2, T3>(this World world) => new Query(world).Exclude<T1, T2, T3>();

		public static Query Exclude<T1, T2, T3, T4>(this World world) => new Query(world).Exclude<T1, T2, T3, T4>();

		public static Query Exclude<T1, T2, T3, T4, T5>(this World world) => new Query(world).Exclude<T1, T2, T3, T4, T5>();

		public static Query Exclude<T1, T2, T3, T4, T5, T6>(this World world) => new Query(world).Exclude<T1, T2, T3, T4, T5, T6>();

		public static Query Exclude<T1, T2, T3, T4, T5, T6, TAnd>(this World world)
			where TAnd : IAndSelector, new() => new Query(world).Exclude<T1, T2, T3, T4, T5, T6, TAnd>();

		public static Query Filter(this World world, Filter filter) => new Query(world, filter);
	}
}
