using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct WorkableVar<T> where T : unmanaged
	{
		public readonly ChunkId ChunkId;

		private readonly Allocator<T> _allocator;

		public WorkableVar(ChunkId chunkId, Allocator<T> allocator)
		{
			ChunkId = chunkId;
			_allocator = allocator;
		}

		public VarHandle<T> Handle
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new VarHandle<T>(ChunkId);
		}

		public ref T Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _allocator.Data[_allocator.Chunks[ChunkId.Id].Offset];
		}
	}
}
