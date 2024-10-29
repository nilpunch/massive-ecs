using System.Runtime.CompilerServices;

namespace Massive
{
	public static class GroupViewExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GroupView Group(this View view, Group group)
		{
			return new GroupView(view.Registry, group);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GroupView Group<TInclude>(this View view)
			where TInclude : IIncludeSelector, new()
		{
			return new GroupView(view.Registry, view.Registry.Group<TInclude, None>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GroupView Group<TInclude, TExclude>(this View view)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			return new GroupView(view.Registry, view.Registry.Group<TInclude, TExclude, None>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GroupView Group<TInclude, TExclude, TOwned>(this View view)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
			where TOwned : IOwnSelector, new()
		{
			return new GroupView(view.Registry, view.Registry.GroupRegistry.Get<TInclude, TExclude, TOwned>());
		}
	}
}
