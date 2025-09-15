#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class SparseSet : BitsBase, IBitsBacktrack
	{
		protected Bits[] RemoveOnAdd { get; private set; } = Array.Empty<Bits>();

		protected int RemoveOnAddCount { get; private set; }

		private Bits[] RemoveOnRemove { get; set; } = Array.Empty<Bits>();

		private int RemoveOnRemoveCount { get; set; }

		/// <summary>
		/// Associated component index. Session-dependent, used for lookups.<br/>
		/// </summary>
		public int ComponentId { get; set; } = -1;

		/// <summary>
		/// Shortcut to access masks.
		/// </summary>
		public Components Components { get; set; }

		/// <summary>
		/// Shoots only after <see cref="Add"/> call, when the ID was not already present.
		/// </summary>
		public event Action<int> AfterAdded;

		/// <summary>
		/// Shoots before each <see cref="Remove"/> call, when the ID was removed.
		/// </summary>
		public event Action<int> BeforeRemoved;

		/// <summary>
		/// Adds an ID. If the ID is already added, no action is performed.
		/// </summary>
		/// <returns>
		/// True if the ID was added; false if it was already present.
		/// </returns>
		/// <remarks>
		/// Throws if provided ID is negative.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Add(int id)
		{
			NegativeArgumentException.ThrowIfNegative(id);

			var id0 = id >> 6;
			var id1 = id >> 12;

			if (id1 >= Bits1.Length)
			{
				Bits1 = Bits1.ResizeToNextPowOf2(id1 + 1);
				Bits0 = Bits0.Resize(Bits1.Length << 6);
			}

			var mod64 = id & 63;
			var bit0 = 1UL << mod64;
			var bit1 = 1UL << (id0 & 63);

			if ((Bits0[id0] & bit0) != 0UL)
			{
				return false;
			}

			if (Bits0[id0] == 0UL)
			{
				EnsurePage(id >> Constants.PageSizePower);
				Bits1[id1] |= bit1;
			}
			Bits0[id0] |= bit0;

			PrepareData(id);

			for (var i = 0; i < RemoveOnAddCount; i++)
			{
				RemoveOnAdd[i].Remove(id);
			}

			Components?.Set(id, ComponentId);
			AfterAdded?.Invoke(id);

			return true;
		}

		/// <summary>
		/// Removes an ID. If the ID is already removed, no action is performed.
		/// </summary>
		/// <returns>
		/// True if the ID was removed; false if it was not present.
		/// </returns>
		/// <remarks>
		/// Throws if provided ID is negative.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(int id)
		{
			NegativeArgumentException.ThrowIfNegative(id);

			var id0 = id >> 6;
			var id1 = id >> 12;

			if (id0 >= Bits0.Length)
			{
				return false;
			}

			var bit0 = 1UL << (id & 63);
			var bit1 = 1UL << (id0 & 63);

			if ((Bits0[id0] & bit0) == 0UL)
			{
				return false;
			}

			BeforeRemoved?.Invoke(id);
			Components?.Remove(id, ComponentId);

			Bits0[id0] &= ~bit0;
			if (Bits0[id0] == 0UL)
			{
				Bits1[id1] &= ~bit1;

				var pageIndex = id >> Constants.PageSizePower;
				var pageMask = Constants.PageMask << ((pageIndex & Constants.PagesInBits1MinusOne) << Constants.PageMaskShiftPower);
				if ((Bits1[id1] & pageMask) == 0UL)
				{
					FreePage(pageIndex);
				}
			}

			for (var i = 0; i < RemoveOnRemoveCount; i++)
			{
				RemoveOnRemove[i].Remove(id);
			}

			return true;
		}

		/// <summary>
		/// Removes all IDs and triggers the <see cref="BeforeRemoved"/> event for each one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			var bits1Length = Bits1.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = Bits1[current1];
				var offset1 = current1 << 6;
				while (bits1 != 0UL)
				{
					var index1 = deBruijn[(int)(((bits1 & (ulong)-(long)bits1) * 0x37E84A99DAE458FUL) >> 58)];

					var current0 = offset1 + index1;
					var bits0 = Bits0[current0];
					var offset0 = current0 << 6;

					while (bits0 != 0UL)
					{
						var index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];

						var id = offset0 + index0;
						BeforeRemoved?.Invoke(id);
						Components?.Remove(id, ComponentId);

						for (var i = 0; i < RemoveOnRemoveCount; i++)
						{
							RemoveOnRemove[i].Remove(id);
						}

						bits0 &= bits0 - 1UL;
					}

					bits1 &= bits1 - 1UL;

					Bits0[current0] = 0UL;
				}
				Bits1[current1] = 0UL;
			}

			FreeAllPages();
		}

		/// <summary>
		/// Removes all IDs without triggering the <see cref="BeforeRemoved"/> event.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClearWithoutNotify()
		{
			Array.Fill(Bits1, 0UL);
			Array.Fill(Bits0, 0UL);
			FreeAllPages();
		}

		/// <summary>
		/// Checks whether the specified ID is present.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(int id)
		{
			var id0 = id >> 6;

			if (id0 < 0 || id0 >= Bits0.Length)
			{
				return false;
			}

			var bit0 = 1UL << (id & 63);
			return (Bits0[id0] & bit0) != 0UL;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitsEnumerator GetEnumerator()
		{
			var bits = BitsPool.RentClone(this).RemoveOnRemove(this);
			return new BitsEnumerator(bits, Bits1.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IBitsBacktrack.PushRemoveOnAdd(Bits bits)
		{
			if (RemoveOnAddCount >= RemoveOnAdd.Length)
			{
				RemoveOnAdd = RemoveOnAdd.ResizeToNextPowOf2(RemoveOnAddCount + 1);
			}

			RemoveOnAdd[RemoveOnAddCount++] = bits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IBitsBacktrack.PopRemoveOnAdd()
		{
			RemoveOnAddCount--;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IBitsBacktrack.PushRemoveOnRemove(Bits bits)
		{
			if (RemoveOnRemoveCount >= RemoveOnRemove.Length)
			{
				RemoveOnRemove = RemoveOnRemove.ResizeToNextPowOf2(RemoveOnRemoveCount + 1);
			}

			RemoveOnRemove[RemoveOnRemoveCount++] = bits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IBitsBacktrack.PopRemoveOnRemove()
		{
			RemoveOnRemoveCount--;
		}

		public virtual void CopyData(int sourceId, int destinationId)
		{
		}

		public virtual void EnsurePage(int page)
		{
		}

		protected virtual void FreePage(int page)
		{
		}

		protected virtual void FreeAllPages()
		{
		}

		protected virtual void PrepareData(int id)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void NotifyAfterAdded(int id)
		{
			AfterAdded?.Invoke(id);
		}

		/// <summary>
		/// Creates and returns a new sparse set that is an exact copy of this one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet CloneBits()
		{
			var clone = new SparseSet();
			CopyBitsTo(clone);
			return clone;
		}
	}
}
