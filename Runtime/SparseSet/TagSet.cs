using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class TagSet : BitSetBase
	{
		public BitSet[] RemoveOnAdd { get; private set; } = Array.Empty<BitSet>();
		public int RemoveOnAddCount { get; private set; }

		public BitSet[] RemoveOnRemove { get; private set; } = Array.Empty<BitSet>();
		public int RemoveOnRemoveCount { get; private set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Add(int id)
		{
			var id0 = id >> 6;
			var id1 = id >> 12;

			if (id1 >= Bits1.Length)
			{
				Bits1 = Bits1.Resize(MathUtils.NextPowerOf2(id1 + 1));
				Bits0 = Bits0.Resize(Bits1.Length << 6);
			}

			var bit0 = 1UL << (id & 63);
			var bit1 = 1UL << (id0 & 63);

			var newPage = -1;
			var alreadyAdded = (Bits0[id0] & bit0) != 0UL;

			if (alreadyAdded)
			{
				return false;
			}

			if (Bits0[id0] == 0UL)
			{
				Bits1[id1] |= bit1;
				newPage = id0;
			}
			Bits0[id0] |= bit0;

			if (newPage > 0)
			{
				AddPage(newPage);
			}

			for (var i = 0; i < RemoveOnAddCount; i++)
			{
				RemoveOnAdd[i].Remove(id);
			}

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(int id)
		{
			var id0 = id >> 6;
			var id1 = id >> 12;

			if (id0 >= Bits0.Length)
			{
				return false;
			}

			var bit0 = 1UL << (id & 63);
			var bit1 = 1UL << (id0 & 63);

			var removedPage = -1;
			var alreadyRemoved = (Bits0[id0] & bit0) == 0UL;

			if (alreadyRemoved)
			{
				return false;
			}

			Bits0[id0] &= ~bit0;
			if (Bits0[id0] == 0UL)
			{
				Bits1[id1] &= ~bit1;
				removedPage = id0;
			}

			if (removedPage > 0)
			{
				RemovePage(removedPage);
			}

			for (var i = 0; i < RemoveOnRemoveCount; i++)
			{
				RemoveOnRemove[i].Remove(id);
			}

			return true;
		}

		protected virtual void AddPage(int page)
		{
		}

		protected virtual void RemovePage(int page)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PushRemoveOnAdd(BitSet bitSetBase)
		{
			if (RemoveOnAddCount >= RemoveOnAdd.Length)
			{
				RemoveOnAdd = RemoveOnAdd.ResizeToNextPowOf2(RemoveOnAddCount + 1);
			}

			RemoveOnAdd[RemoveOnAddCount++] = bitSetBase;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopRemoveOnAdd()
		{
			RemoveOnAddCount--;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PushRemoveOnRemove(BitSet bitSetBase)
		{
			if (RemoveOnRemoveCount >= RemoveOnRemove.Length)
			{
				RemoveOnRemove = RemoveOnRemove.ResizeToNextPowOf2(RemoveOnRemoveCount + 1);
			}

			RemoveOnRemove[RemoveOnRemoveCount++] = bitSetBase;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopRemoveOnRemove()
		{
			RemoveOnRemoveCount--;
		}
	}
}
