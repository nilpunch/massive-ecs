using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct BitsEnumerator : IDisposable
	{
		private readonly Bits _rentedBits;
		private readonly Pops _rentedPops;

		private readonly int _bits1Length;

		private ulong _bits1;
		private int _current1;
		private int _offset1;

		private ulong _bits0;
		private int _current0;
		private int _offset0;

		public BitsEnumerator(Bits rentedBits, Pops rentedPops, int bits1Length)
		{
			_rentedBits = rentedBits;
			_rentedPops = rentedPops;
			_bits1Length = bits1Length;

			_bits1 = 0UL;
			for (_current1 = 0; _current1 < _bits1Length; _current1++)
			{
				_bits1 = _rentedBits.Bits1[_current1];
				if (_bits1 != 0UL)
				{
					break;
				}
			}

			_offset1 = _current1 << 6;
			_current0 = _offset1 + MathUtils.LSB(_bits1);
			_bits1 &= _bits1 - 1UL;
			_offset0 = _current0 << 6;
			_bits0 = _rentedBits.Bits0[_current0];

			Current = default;
		}

		public int Current { get; private set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (_bits1Length == 0)
			{
				return false;
			}

			_bits0 &= _rentedBits.Bits0[_current0];
			if (_bits0 != 0UL)
			{
				Current = _offset0 + MathUtils.LSB(_bits0);
				_bits0 &= _bits0 - 1UL;
				return true;
			}

			_bits1 &= _rentedBits.Bits1[_current1];
			if (_bits1 != 0UL)
			{
				_current0 = _offset1 + MathUtils.LSB(_bits1);
				_bits1 &= _bits1 - 1UL;
				_offset0 = _current0 << 6;
				_bits0 = _rentedBits.Bits0[_current0];

				Current = _offset0 + MathUtils.LSB(_bits0);
				_bits0 &= _bits0 - 1UL;
				return true;
			}

			while (++_current1 < _bits1Length && _rentedBits.Bits1[_current1] == 0UL)
			{
			}

			if (_current1 >= _bits1Length)
			{
				return false;
			}

			_offset1 = _current1 << 6;
			_bits1 = _rentedBits.Bits1[_current1];

			_current0 = _offset1 + MathUtils.LSB(_bits1);
			_bits1 &= _bits1 - 1UL;
			_offset0 = _current0 << 6;
			_bits0 = _rentedBits.Bits0[_current0];

			Current = _offset0 + MathUtils.LSB(_bits0);
			_bits0 &= _bits0 - 1UL;
			return true;
		}

		public void Dispose()
		{
			BitsPool.Return(_rentedBits);
			PopsPool.ReturnAndPop(_rentedPops);
		}
	}
}
