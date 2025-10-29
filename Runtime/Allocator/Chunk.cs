using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Massive
{
	[StructLayout(LayoutKind.Explicit, Size = Size)]
	public struct Chunk
	{
		public const int Size = 16;
		public const int Alignment = 4;

		[FieldOffset(0)] public int OffsetInBytes;

		/// <summary>
		/// Used for active chunks. Negative when inactive.
		/// </summary>
		[FieldOffset(4)] public int LengthInBytes;

		/// <summary>
		/// Used for inactive chunks. Stored as complement to the actual ID.
		/// </summary>
		[FieldOffset(4)] public int NextFreeId;

		/// <summary>
		/// Chunks with version 0 are invalid.
		/// </summary>
		[FieldOffset(8)] public uint Version;

		[FieldOffset(12)] public int AlignedOffsetInBytes;

		public static Chunk DefaultValid
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				var chunk = default(Chunk);
				chunk.Version = 1;
				return chunk;
			}
		}
	}
}
