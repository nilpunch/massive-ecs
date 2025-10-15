#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Copying extension for <see cref="Massive.DataSet{T}"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class CopyingDataSet<T> : DataSet<T> where T : ICopyable<T>
	{
		public CopyingDataSet(T defaultValue = default) : base(defaultValue)
		{
		}

		/// <summary>
		/// Copies the data from one index to another, using <see cref="ICopyable{T}"/>.
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
			var clone = new CopyingDataSet<T>(DefaultValue);
			CopyToCopyable(clone);
			return clone;
		}

		/// <summary>
		/// Copies all data and bitset state from this set into the specified one.
		/// All data is copied using <see cref="ICopyable{T}"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyToCopyable(DataSet<T> other)
		{
			CopyBitSetTo(other);

			var blocksLength = BlocksCapacity;

			var deBruijn = MathUtils.DeBruijn;
			for (var blockIndex = 0; blockIndex < blocksLength; blockIndex++)
			{
				var block = NonEmptyBlocks[blockIndex];
				var blockOffset = blockIndex << 6;
				while (block != 0UL)
				{
					var blockBit = (int)deBruijn[(int)(((block & (ulong)-(long)block) * 0x37E84A99DAE458FUL) >> 58)];

					var bitsIndex = blockOffset + blockBit;
					var bitsOffset = bitsIndex << 6;
					var dataOffset = bitsOffset & Constants.PageSizeMinusOne;
					var pageIndex = bitsOffset >> Constants.PageSizePower;

					other.EnsurePageInternal(pageIndex);
					var sourcePage = PagedData[pageIndex];
					var destinationPage = other.PagedData[pageIndex];

					var bits = Bits[bitsIndex];
					while (bits != 0UL)
					{
						var bit = (int)deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
						sourcePage[dataOffset + bit].CopyTo(ref destinationPage[dataOffset + bit]);
						bits &= bits - 1UL;
					}

					block &= block - 1UL;
				}
			}
		}
	}
}
