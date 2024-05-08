using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistryGroupExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IGroup Group(this IRegistry registry, IReadOnlyList<ISet> owned = null,
			IReadOnlyList<IReadOnlySet> include = null, IReadOnlyList<IReadOnlySet> exclude = null)
		{
			return registry.Groups.EnsureGroup(owned, include, exclude);
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
