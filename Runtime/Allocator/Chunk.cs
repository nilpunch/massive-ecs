using System.Runtime.InteropServices;

namespace Massive
{
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	public struct Chunk
	{
		[FieldOffset(0)] public int Offset;

		/// <summary>
		/// Used for active chunks. Negative when inactive.
		/// </summary>
		[FieldOffset(4)] public int Length;

		/// <summary>
		/// Used for inactive chunks. Stored as complement to the actual ID.
		/// </summary>
		[FieldOffset(4)] public int NextFreeId;

		/// <summary>
		/// Chunks with version 0 are invalid.
		/// </summary>
		[FieldOffset(8)] public uint Version;

		/// <summary>
		/// Chunk that will be dealloced when this chunk is dealloced.
		/// </summary>
		[FieldOffset(12)] public int ChildChunkId;
	}
}
