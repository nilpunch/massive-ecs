using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks | Option.DivideByZeroChecks, false)]
	public class BitsetSet
	{
		private readonly int _bitsPerElement;
		private readonly int _bitsPerBitset;

		private short[] _data;

		public int BitsetSize { get; }
		public int ElementSize { get; }

		public BitsetSet(int bitsPerElement = Constants.DefaultBitsPerElement, int bitsPerBitset = Constants.DefaultBitsPerBitset, int capacity = Constants.DefaultCapacity)
		{
			if (bitsPerBitset <= 0 || bitsPerBitset % 16 != 0)
			{
				throw new ArgumentOutOfRangeException(nameof(bitsPerBitset), bitsPerBitset, "Value must be positive and divisible by 16!");
			}

			_bitsPerElement = bitsPerElement;
			_bitsPerBitset = bitsPerBitset;
			BitsetSize = bitsPerBitset / 16;
			ElementSize = BitsetSize + 1 + bitsPerElement;
			_data = new short[ElementSize * capacity];
		}

		public short[] Data => _data;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void Resize(int capacity)
		{
			Array.Resize(ref _data, ElementSize * capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AssignBit(int id, int bit)
		{
			// if (bit >= _bitsPerBitset)
			// {
			// 	throw new InvalidOperationException();
			// }
			//
			// if (IsBitAssigned(id, bit))
			// {
			// 	return;
			// }

			int elementIndex = GetElementIndex(id);
			// if (_data[elementIndex + BitsetSize] == _bitsPerElement)
			// {
			// 	throw new InvalidOperationException();
			// }

			// Flip bit in bitset
			int bitsetElementIndex = elementIndex + BitsetElementIndex(bit);
			int mask = 1 << BitsetBitIndex(bit);
			_data[bitsetElementIndex] |= (short)mask;

			// Append this bit to the bits
			int countIndex = elementIndex + BitsetSize;
			_data[countIndex + 1 + _data[countIndex]] = (short)bit;
			_data[countIndex]++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void UnassignBit(int id, int bit)
		{
			// if (bit >= _bitsPerBitset)
			// {
			// 	throw new InvalidOperationException();
			// }
			//
			// if (!IsBitAssigned(id, bit))
			// {
			// 	return;
			// }

			int elementIndex = GetElementIndex(id);

			// Flip bit in bitset
			int bitsetElementIndex = elementIndex + BitsetElementIndex(bit);
			int mask = 1 << BitsetBitIndex(bit);
			_data[bitsetElementIndex] &= (short)~mask;

			// Swap remove bit
			int countIndex = elementIndex + BitsetSize;
			_data[countIndex] -= 1;
			int setsStart = countIndex + 1;
			int setsEnd = setsStart + _data[countIndex];

			for (int i = setsStart; i <= setsEnd; i++)
			{
				if (_data[i] == bit)
				{
					_data[i] = _data[setsEnd];
					return;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void UnassignAllBits(int id)
		{
			int elementIndex = GetElementIndex(id);
			// if (_data[elementIndex + BitsetSize] == _bitsPerElement)
			// {
			// 	throw new InvalidOperationException();
			// }

			// Zeroing bitset
			for (int i = 0; i < BitsetSize; i++)
			{
				_data[elementIndex + i] = 0;
			}
			_data[elementIndex] = 0;

			// Zeroing bits count
			_data[elementIndex + BitsetSize] = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan<short> GetAllBits(int id)
		{
			int elementIndex = GetElementIndex(id);
			if (elementIndex < 0 || elementIndex >= _data.Length)
			{
				return ReadOnlySpan<short>.Empty;
			}

			return new ReadOnlySpan<short>(_data, elementIndex + BitsetSize + 1, _data[elementIndex + BitsetSize]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsBitAssigned(int id, int bit)
		{
			int elementIndex = GetElementIndex(id);
			if (elementIndex < 0 || elementIndex >= _data.Length)
			{
				return false;
			}

			int bitsetElementIndex = elementIndex + BitsetElementIndex(bit);
			int mask = 1 << BitsetBitIndex(bit);
			return (_data[bitsetElementIndex] & mask) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementIndex(int id)
		{
			return id * ElementSize;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int BitsetElementIndex(int bit)
		{
			return MathHelpers.FastPowDiv(bit, 4); // return bit / 16;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int BitsetBitIndex(int bit)
		{
			const int bitsetElementSize = sizeof(short) * 8;
			return MathHelpers.FastMod(bit, bitsetElementSize); // return bit % 16;
		}
	}
}
