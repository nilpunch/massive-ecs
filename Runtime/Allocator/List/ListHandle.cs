using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly partial struct ListHandle<T> where T : unmanaged
	{
		public readonly ArrayHandle<T> ItemsId;
		public readonly VarHandleInt CountId;

		public ListHandle(ArrayHandle<T> itemsId, VarHandleInt countId)
		{
			ItemsId = itemsId;
			CountId = countId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> In(Allocator allocator)
		{
			return new WorkableList<T>(this, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ListId(ListHandle<T> handle)
		{
			return new ListId(handle.ItemsId, handle.CountId);
		}
	}
}
