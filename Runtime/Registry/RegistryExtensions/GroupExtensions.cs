using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class GroupExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Group Group<TInclude>(this Registry registry)
			where TInclude : IIncludeSelector, new()
		{
			return registry.GroupRegistry.Get<TInclude, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Group Group<TInclude, TExclude>(this Registry registry)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			return registry.GroupRegistry.Get<TInclude, TExclude>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Group Group(this Registry registry, SparseSet[] included = null, SparseSet[] excluded = null)
		{
			return registry.GroupRegistry.Get(included, excluded);
		}
	}
}
