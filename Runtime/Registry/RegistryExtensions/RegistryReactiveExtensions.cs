using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class RegistryReactiveExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReactiveFilter ReactiveFilter<TInclude>(this Registry registry)
			where TInclude : IIncludeSelector, new()
		{
			return registry.ReactiveRegistry.Get<TInclude, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReactiveFilter ReactiveFilter<TInclude, TExclude>(this Registry registry)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			return registry.ReactiveRegistry.Get<TInclude, TExclude>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReactiveFilter ReactiveFilter(this Registry registry, SparseSet[] included = null, SparseSet[] excluded = null)
		{
			return registry.ReactiveRegistry.Get(included, excluded);
		}
	}
}
