using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistryViewExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static View View(this IRegistry registry)
		{
			return new View(registry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static View<T> View<T>(this IRegistry registry)
		{
			return new View<T>(registry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static View<T1, T2> View<T1, T2>(this IRegistry registry)
		{
			return new View<T1, T2>(registry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static View<T1, T2, T3> View<T1, T2, T3>(this IRegistry registry)
		{
			return new View<T1, T2, T3>(registry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView FilterView(this IRegistry registry, IFilter filter = null)
		{
			return new FilterView(registry, filter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView<T> FilterView<T>(this IRegistry registry, IFilter filter = null)
		{
			return new FilterView<T>(registry, filter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView<T1, T2> FilterView<T1, T2>(this IRegistry registry, IFilter filter = null)
		{
			return new FilterView<T1, T2>(registry, filter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView<T1, T2, T3> FilterView<T1, T2, T3>(this IRegistry registry, IFilter filter = null)
		{
			return new FilterView<T1, T2, T3>(registry, filter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GroupView GroupView(this IRegistry registry, IGroup group)
		{
			return new GroupView(group);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GroupView<T> GroupView<T>(this IRegistry registry, IGroup group)
		{
			return new GroupView<T>(registry, group);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GroupView<T1, T2> GroupView<T1, T2>(this IRegistry registry, IGroup group)
		{
			return new GroupView<T1, T2>(registry, group);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GroupView<T1, T2, T3> GroupView<T1, T2, T3>(this IRegistry registry, IGroup group)
		{
			return new GroupView<T1, T2, T3>(registry, group);
		}
	}
}
