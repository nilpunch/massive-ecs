using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct GroupView : IView
	{
		private readonly IGroup _group;

		public GroupView(IGroup group)
		{
			_group = group;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachUniversal<TInvoker>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker
		{
			_group.EnsureSynced();

			var groupIds = _group.Ids;
			for (var i = groupIds.Length - 1; i >= 0; i--)
			{
				invoker.Apply(groupIds[i]);
			}
		}
	}
}
