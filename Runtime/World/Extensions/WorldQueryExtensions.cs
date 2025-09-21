using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldQueryExtensions
	{
		public static Query All<T>(this World world) => new Query(world).All<T>();

		public static Query All<T1, T2>(this World world) => new Query(world).All<T1, T2>();

		public static Query All<T1, T2, T3>(this World world) => new Query(world).All<T1, T2, T3>();
		
		public static Query All<T1, T2, T3, T4>(this World world) => new Query(world).All<T1, T2, T3, T4>();
		
		public static Query All<T1, T2, T3, T4, T5>(this World world) => new Query(world).All<T1, T2, T3, T4, T5>();
		
		public static Query All<T1, T2, T3, T4, T5, T6>(this World world) => new Query(world).All<T1, T2, T3, T4, T5, T6>();

		public static Query All<T1, T2, T3, T4, T5, T6, TAnd>(this World world)
			where TAnd : IAndSelector, new() => new Query(world).All<T1, T2, T3, T4, T5, T6, TAnd>();

		public static Query None<T>(this World world) => new Query(world).None<T>();

		public static Query None<T1, T2>(this World world) => new Query(world).None<T1, T2>();

		public static Query None<T1, T2, T3>(this World world) => new Query(world).None<T1, T2, T3>();
		
		public static Query None<T1, T2, T3, T4>(this World world) => new Query(world).None<T1, T2, T3, T4>();
		
		public static Query None<T1, T2, T3, T4, T5>(this World world) => new Query(world).None<T1, T2, T3, T4, T5>();
		
		public static Query None<T1, T2, T3, T4, T5, T6>(this World world) => new Query(world).None<T1, T2, T3, T4, T5, T6>();

		public static Query None<T1, T2, T3, T4, T5, T6, TAnd>(this World world)
			where TAnd : IAndSelector, new() => new Query(world).None<T1, T2, T3, T4, T5, T6, TAnd>();

		public static Query Filter(this World world, Filter filter) => new Query(world, filter);
	}
}
