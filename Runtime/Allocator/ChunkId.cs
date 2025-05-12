using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly struct ChunkId
	{
		/// <summary>
		/// 0 counted as invalid.
		/// [ AllocatorTypeId: 16 bits | Version: 16 bits | ID: 32 bits ]
		/// </summary>
		public readonly long AllocatorVersionId;

		public int Id
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (int)AllocatorVersionId;
		}

		/// <summary>
		/// Chunks with version 0 are invalid.
		/// </summary>
		public ushort Version
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (ushort)(AllocatorVersionId >> 32);
		}

		public ushort AllocatorTypeId
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (ushort)(AllocatorVersionId >> 48);
		}

		private ChunkId(long allocatorVersionId)
		{
			AllocatorVersionId = allocatorVersionId;
		}

		/// [ AllocatorTypeId: 16 bits | Version: 16 bits | ID: 32 bits ]
		public ChunkId(int id, ushort version, ushort allocatorTypeId)
		{
			AllocatorVersionId = (uint)id | ((long)version << 32) | ((long)allocatorTypeId << 48);
		}

		public static ChunkId Invalid
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ChunkId(0);
		}

		public bool IsValid
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => AllocatorVersionId > 0;
		}
	}
}
