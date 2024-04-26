using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct GroupView
	{
		private readonly IGroup _group;

		public GroupView(IGroup group)
		{
			_group = group;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityAction action)
		{
			var groupIds = _group.Ids;
			for (var i = groupIds.Length - 1; i >= 0; i--)
			{
				action.Invoke(groupIds[i]);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionExtra<TExtra> action)
		{
			var groupIds = _group.Ids;
			for (var i = groupIds.Length - 1; i >= 0; i--)
			{
				action.Invoke(groupIds[i], extra);
			}
		}
	}
}
