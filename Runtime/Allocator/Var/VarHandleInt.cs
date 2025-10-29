using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly partial struct VarHandleInt
	{
		public readonly ChunkId ChunkId;

		public VarHandleInt(ChunkId chunkId)
		{
			ChunkId = chunkId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVarInt In(World world)
		{
			return new WorkableVarInt(this, world.Allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVarInt In(Allocator allocator)
		{
			return new WorkableVarInt(this, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ChunkId(VarHandleInt handle)
		{
			return handle.ChunkId;
		}
	}
}
