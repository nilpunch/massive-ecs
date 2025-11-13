using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct WorkableVarInt
	{
		private readonly VarHandleInt _varHandle;
		private readonly Allocator _allocator;

		public WorkableVarInt(VarHandleInt varHandle, Allocator allocator)
		{
			_varHandle = varHandle;
			_allocator = allocator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator VarHandleInt(WorkableVarInt workableVar)
		{
			return workableVar._varHandle;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free()
		{
			_varHandle.Free(_allocator);
		}

		public ref int Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _varHandle.Value(_allocator);
		}
	}
}
