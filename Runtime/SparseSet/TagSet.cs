using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class TagSet
	{
		public BitSet BitSet { get; } = new BitSet();

		public BitSet[] RemoveOnAdd { get; private set; } = Array.Empty<BitSet>();
		public int RemoveOnAddCount { get; private set; }

		public BitSet[] RemoveOnRemove { get; private set; } = Array.Empty<BitSet>();
		public int RemoveOnRemoveCount { get; private set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(int id)
		{
			var newPage = BitSet.Add(id);

			if (newPage > 0)
			{
				AddPage(newPage);
			}

			for (var i = 0; i < RemoveOnAddCount; i++)
			{
				RemoveOnAdd[i].Remove(id);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int id)
		{
			var removedPage = BitSet.Remove(id);

			if (removedPage > 0)
			{
				RemovePage(removedPage);
			}

			for (var i = 0; i < RemoveOnRemoveCount; i++)
			{
				RemoveOnRemove[i].Remove(id);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(int id)
		{
			return BitSet.Has(id);
		}

		protected virtual void AddPage(int page)
		{
		}

		protected virtual void RemovePage(int page)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PushRemoveOnAdd(BitSet bitSet)
		{
			if (RemoveOnAddCount >= RemoveOnAdd.Length)
			{
				RemoveOnAdd = RemoveOnAdd.ResizeToNextPowOf2(RemoveOnAddCount + 1);
			}

			RemoveOnAdd[RemoveOnAddCount++] = bitSet;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopRemoveOnAdd()
		{
			RemoveOnAddCount--;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PushRemoveOnRemove(BitSet bitSet)
		{
			if (RemoveOnRemoveCount >= RemoveOnRemove.Length)
			{
				RemoveOnRemove = RemoveOnRemove.ResizeToNextPowOf2(RemoveOnRemoveCount + 1);
			}

			RemoveOnRemove[RemoveOnRemoveCount++] = bitSet;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopRemoveOnRemove()
		{
			RemoveOnRemoveCount--;
		}
	}
}
