using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Massive
{
	[StructLayout(LayoutKind.Explicit, Size = 12)]
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
