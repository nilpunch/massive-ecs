using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class WorldGroupExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Group Group<TInclude>(this World world)
			where TInclude : IIncludeSelector, new()
		{
			return world.GroupRegistry.Get<TInclude, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Group Group<TInclude, TExclude>(this World world)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			return world.GroupRegistry.Get<TInclude, TExclude>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Group Group(this World world, SparseSet[] included = null, SparseSet[] excluded = null)
		{
			return world.GroupRegistry.Get(included, excluded);
		}
	}
}
