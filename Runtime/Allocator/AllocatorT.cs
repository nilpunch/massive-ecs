using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public sealed class Allocator<T> : Allocator where T : unmanaged
	{
		public T DefaultValue { get; }

		public T[] Data { get; private set; } = Array.Empty<T>();

		private int DataCapacity { get; set; }

		public Allocator(T defaultValue = default)
			: base(AllocatorId<T>.Index)
		{
			DefaultValue = defaultValue;
		}

		public override void EnsureDataCapacity(int capacity)
		{
			if (capacity > DataCapacity)
			{
				DataCapacity = MathUtils.NextPowerOf2(capacity);
				Data = Data.Resize(DataCapacity);
			}
		}

		protected override void CopyData(int source, int destination, int length)
		{
			Array.Copy(Data, source, Data, destination, length);
		}

		protected override void ClearData(int start, int length)
		{
			Array.Fill(Data, DefaultValue, start, length);
		}

		public override Type ElementType => typeof(T);

		public override Array RawData => Data;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Allocator<T> Clone()
		{
			var clone = new Allocator<T>();
			CopyTo(clone);
			return clone;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(Allocator<T> other)
		{
			other.EnsureDataCapacity(UsedSpace);

			Array.Copy(Chunks, other.Chunks, ChunkCount);
			Array.Copy(ChunkFreeLists, other.ChunkFreeLists, FreeListsLength);
			Array.Copy(Data, other.Data, UsedSpace);

			if (ChunkCount < other.ChunkCount)
			{
				Array.Fill(other.Chunks, Chunk.DefaultValid, ChunkCount, other.ChunkCount - ChunkCount);
			}

			other.SetState(ChunkCount, UsedSpace);
		}
	}
}
