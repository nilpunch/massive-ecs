using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistryGroupExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IGroup Group<TOwned>(this IRegistry registry) where TOwned : struct, ISetSelector
		{
			return registry.Group<TOwned, None, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IGroup Group<TOwned, TInclude>(this IRegistry registry) where TOwned : struct, ISetSelector
			where TInclude : struct, IReadOnlySetSelector
		{
			return registry.Group<TOwned, TInclude, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IGroup Group<TOwned, TInclude, TExclude>(this IRegistry registry) where TOwned : struct, ISetSelector
			where TInclude : struct, IReadOnlySetSelector
			where TExclude : struct, IReadOnlySetSelector
		{
			return registry.Groups.EnsureGroup(new GroupSelector<TOwned, TInclude, TExclude>(registry));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<ISet> Many<T1>(this IRegistry registry)
		{
			return new[] { registry.Any<T1>() };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<ISet> Many<T1, T2>(this IRegistry registry)
		{
			return new[] { registry.Any<T1>(), registry.Any<T2>() };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<ISet> Many<T1, T2, T3>(this IRegistry registry)
		{
			return new[] { registry.Any<T1>(), registry.Any<T2>(), registry.Any<T3>() };
		}
	}
}
