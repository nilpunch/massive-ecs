using System.Runtime.CompilerServices;

namespace Massive
{
	public struct ChunkId
	{
		/// <summary>
		/// 0 counted as invalid.
		/// </summary>
		public long IdAndVersion;

		public int Id
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (int)IdAndVersion;
		}

		/// <summary>
		/// Chunks with version 0 are invalid.
		/// </summary>
		public uint Version
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (uint)(IdAndVersion >> 32);
		}

		private ChunkId(long idAndVersion)
		{
			IdAndVersion = idAndVersion;
		}

		public ChunkId(int id, uint version)
		{
			IdAndVersion = (uint)id | ((long)version << 32);
		}

		public static ChunkId Invalid
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ChunkId(0);
		}

		public bool IsValid
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => IdAndVersion > 0;
		}
	}
}
