using System.Runtime.CompilerServices;

namespace Massive
{
	public static class GroupViewExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GroupView Group(this View view, IGroup group)
		{
			return new GroupView(view.Registry, group);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GroupView Group<TOwned>(this View view)
			where TOwned : IOwnSelector, new()
		{
			return new GroupView(view.Registry, view.Registry.Group<TOwned>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GroupView Group<TOwned, TInclude>(this View view)
			where TOwned : IOwnSelector, new()
			where TInclude : IIncludeSelector, new()
		{
			return new GroupView(view.Registry, view.Registry.Group<TOwned, TInclude>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GroupView Group<TOwned, TInclude, TExclude>(this View view)
			where TOwned : IOwnSelector, new()
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			return new GroupView(view.Registry, view.Registry.GroupRegistry.Get<TOwned, TInclude, TExclude>());
		}
	}
}
