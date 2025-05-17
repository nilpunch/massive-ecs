using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldFilterExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Filter Filter<TInclude>(this World world)
			where TInclude : IIncludeSelector, new()
		{
			return world.Filters.Get<TInclude, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Filter Filter<TInclude, TExclude>(this World world)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			return world.Filters.Get<TInclude, TExclude>();
		}
	}
}
