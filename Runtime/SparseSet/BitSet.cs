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
	public class BitSet : BitsBase
	{
		/// <summary>
		/// Associated component index. Session-dependent, used for lookups.<br/>
		/// </summary>
		public int ComponentId { get; set; } = -1;

		/// <summary>
		/// Shortcut to access masks.
		/// </summary>
		public Masks Masks { get; set; }

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
				Bits1 = Bits1.Resize(MathUtils.NextPowerOf2(id1 + 1));
				Bits0 = Bits0.Resize(Bits1.Length << 6);
			}

			var bit0 = 1UL << (id & 63);
			var bit1 = 1UL << (id0 & 63);

			var newPage = -1;
			var alreadyPresent = (Bits0[id0] & bit0) != 0UL;

			if (alreadyPresent)
			{
				return false;
			}

			if (Bits0[id0] == 0UL)
			{
				Bits1[id1] |= bit1;
				newPage = id0;
			}
			Bits0[id0] |= bit0;

			if (newPage >= 0)
			{
				AddPage(newPage);
			}

			for (var i = 0; i < RemoveOnAddCount; i++)
			{
				RemoveOnAdd[i].Remove(id);
			}

			AfterAdded?.Invoke(id);
			Masks?.Set(id, ComponentId);

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

			var removedPage = -1;
			var notPresent = (Bits0[id0] & bit0) == 0UL;

			if (notPresent)
			{
				return false;
			}

			BeforeRemoved?.Invoke(id);
			Masks?.Remove(id, ComponentId);

			Bits0[id0] &= ~bit0;
			if (Bits0[id0] == 0UL)
			{
				Bits1[id1] &= ~bit1;
				removedPage = id0;
			}

			if (removedPage >= 0)
			{
				RemovePage(removedPage);
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
			throw new NotImplementedException();
		}

		/// <summary>
		/// Removes all IDs without triggering the <see cref="BeforeRemoved"/> event.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClearWithoutNotify()
		{
			throw new NotImplementedException();
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

		/// <summary>
		/// Ensures the sparse array has sufficient capacity for the specified index.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureBitsAt(int index)
		{
			var id1 = index >> 12;

			if (id1 >= Bits1.Length)
			{
				Bits1 = Bits1.Resize(MathUtils.NextPowerOf2(id1 + 1));
				Bits0 = Bits0.Resize(Bits1.Length << 6);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitsEnumerator GetEnumerator()
		{
			var bits = BitsPool.RentClone(this);
			PushRemoveOnRemove(bits);
			var pops = PopsPool.Rent().AddPopOnRemove(this);
			return new BitsEnumerator(bits, pops, Bits1.Length);
		}

		protected virtual void AddPage(int page)
		{
		}

		protected virtual void RemovePage(int page)
		{
		}

		/// <summary>
		/// Prepares data at the specified index, if necessary.
		/// </summary>
		protected virtual void PrepareData(int id)
		{
		}

		public virtual void CopyData(int sourceId, int destinationId)
		{
			throw new NotImplementedException();
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
		public BitSet CloneBits()
		{
			var clone = new BitSet();
			CopyBitsTo(clone);
			return clone;
		}
	}
}
