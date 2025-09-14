#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Copying extension for <see cref="Massive.ManagedDataSet{T}"/>.
	/// Swaps data when elements are moved.
	/// Used for managed components to reduce allocations.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class CopyingDataSet<T> : DataSet<T> where T : ICopyable<T>
	{
		public CopyingDataSet(int pageSize = Constants.DefaultPageSize)
			: base(pageSize)
		{
		}

		/// <summary>
		/// Copies the data from one index to another.
		/// </summary>
		public override void CopyData(int sourceId, int destinationId)
		{
			Get(sourceId).CopyTo(ref Get(destinationId));
		}

		/// <summary>
		/// Creates and returns a new copying data set that is an exact copy of this one.
		/// All data is copied using <see cref="ICopyable{T}"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CopyingDataSet<T> CloneCopyable()
		{
			var clone = new CopyingDataSet<T>(PageSize);
			CopyToCopyable(clone);
			return clone;
		}

		/// <summary>
		/// Copies all data and sparse state from this set into the specified one.
		/// All data is copied using <see cref="ICopyable{T}"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyToCopyable(DataSet<T> other)
		{
			IncompatiblePageSizeException.ThrowIfIncompatible(this, other);

			CopyBitsTo(other);

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
					var sourceOffset = current0 & PageSizeMinusOne;
					var destinationOffset = current0 & other.PageSizeMinusOne;
					var sourcePage = PagedData[current0 >> PageSizePower];
					other.EnsurePage(current0 >> other.PageSizePower);
					var destinationPage = other.PagedData[current0 >> other.PageSizePower];
					var bits0 = Bits0[current0];
					while (bits0 != 0UL)
					{
						var index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];
						sourcePage[sourceOffset + index0].CopyTo(ref destinationPage[destinationOffset + index0]);
						bits0 &= bits0 - 1UL;
					}

					bits1 &= bits1 - 1UL;
				}
			}
		}
	}
}
