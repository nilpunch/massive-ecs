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

		private readonly int _bits1Length;
		private readonly byte[] _deBruijn;

		private int _current1;
		private int _offset1;
		private ulong _bits1;

		private int _current0;
		private int _offset0;
		private ulong _bits0;

		public BitsEnumerator(Bits rentedBits, int bits1Length)
		{
			_rentedBits = rentedBits;
			_bits1Length = bits1Length;

			_current1 = -1;
			_offset1 = default;
			_bits1 = default;
			_current0 = default;
			_offset0 = default;
			_bits0 = default;

			_deBruijn = MathUtils.DeBruijn;

			Current = default;

			while (++_current1 < _bits1Length)
			{
				if (_rentedBits.Bits1[_current1] != 0UL)
				{
					_offset1 = _current1 << 6;
					_bits1 = _rentedBits.Bits1[_current1];

					_current0 = _offset1 + _deBruijn[(int)(((_bits1 & (ulong)-(long)_bits1) * 0x37E84A99DAE458FUL) >> 58)];
					_bits1 &= _bits1 - 1UL;
					_offset0 = _current0 << 6;

					_bits0 = _rentedBits.Bits0[_current0];
					return;
				}
			}

			_bits1Length = 0;
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
				Current = _offset0 + _deBruijn[(int)(((_bits0 & (ulong)-(long)_bits0) * 0x37E84A99DAE458FUL) >> 58)];
				_bits0 &= _bits0 - 1UL;
				return true;
			}

			_bits1 &= _rentedBits.Bits1[_current1];
			if (_bits1 != 0UL)
			{
				_current0 = _offset1 + _deBruijn[(int)(((_bits1 & (ulong)-(long)_bits1) * 0x37E84A99DAE458FUL) >> 58)];
				_bits1 &= _bits1 - 1UL;
				_offset0 = _current0 << 6;

				_bits0 = _rentedBits.Bits0[_current0];
				Current = _offset0 + _deBruijn[(int)(((_bits0 & (ulong)-(long)_bits0) * 0x37E84A99DAE458FUL) >> 58)];
				_bits0 &= _bits0 - 1UL;
				return true;
			}

			while (++_current1 < _bits1Length)
			{
				if (_rentedBits.Bits1[_current1] != 0UL)
				{
					_offset1 = _current1 << 6;
					_bits1 = _rentedBits.Bits1[_current1];

					_current0 = _offset1 + _deBruijn[(int)(((_bits1 & (ulong)-(long)_bits1) * 0x37E84A99DAE458FUL) >> 58)];
					_bits1 &= _bits1 - 1UL;
					_offset0 = _current0 << 6;

					_bits0 = _rentedBits.Bits0[_current0];
					Current = _offset0 + _deBruijn[(int)(((_bits0 & (ulong)-(long)_bits0) * 0x37E84A99DAE458FUL) >> 58)];
					_bits0 &= _bits0 - 1UL;
					return true;
				}
			}

			return false;
		}

		public void Dispose()
		{
			BitsPool.ReturnAndPop(_rentedBits);
		}
	}
}
