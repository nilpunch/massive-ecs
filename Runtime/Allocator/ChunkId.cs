using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly struct ChunkId
	{
		/// <summary>
		/// 0 counted as invalid.<br/>
		/// [ Version: 32 bits | ID: 32 bits ]
		/// </summary>
		public readonly long VersionId;

		public int Id
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (int)VersionId;
		}

		/// <summary>
		/// Chunks with version 0 are invalid.
		/// </summary>
		public uint Version
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (uint)(VersionId >> 32);
		}

		/// <summary>
		/// [ Version: 32 bits | ID: 32 bits ]
		/// </summary>
		public ChunkId(int id, uint version)
		{
			VersionId = (uint)id | ((long)version << 32);
		}
	}
}
