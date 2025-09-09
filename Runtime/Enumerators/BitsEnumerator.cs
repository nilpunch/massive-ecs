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

		private int _current1;
		private int _offset1;
		private ulong _bits1;
		private int _index1;
		private int _runEnd1;

		private int _current0;
		private int _offset0;
		private ulong _bits0;
		private int _index0;
		private int _runEnd0;

		public BitsEnumerator(Bits rentedBits, int bits1Length)
		{
			_rentedBits = rentedBits;
			_bits1Length = bits1Length;

			_current1 = -1;
			_offset1 = default;
			_bits1 = default;
			_index1 = default;
			_runEnd1 = default;
			_current0 = default;
			_offset0 = default;
			_bits0 = default;
			_index0 = default;
			_runEnd0 = default;

			Current = default;
		}

		public int Current { get; private set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			while (++_index0 < _runEnd0)
			{
				if ((_rentedBits.Bits0[_current0] & (1UL << _index0)) != 0UL)
				{
					Current = _offset0 + _index0;
					return true;
				}
			}

			if (_bits0 != 0UL)
			{
				_bits0 &= _rentedBits.Bits0[_current0];
				if (_bits0 != 0UL)
				{
					Current = _offset0 + MathUtils.LSB(_bits0);
					_bits0 &= _bits0 - 1UL;
					return true;
				}
			}

			while (++_index1 < _runEnd1)
			{
				if ((_rentedBits.Bits1[_current1] & (1UL << _index1)) != 0UL)
				{
					_current0 = _offset1 + _index1;
					_offset0 = _current0 << 6;
					SetupAndRunInnerLoop();
					return true;
				}
			}

			if (_bits1 != 0UL)
			{
				_bits1 &= _rentedBits.Bits1[_current1];
				if (_bits1 != 0UL)
				{
					_current0 = _offset1 + MathUtils.LSB(_bits1);
					_bits1 &= _bits1 - 1UL;
					_offset0 = _current0 << 6;
					SetupAndRunInnerLoop();
					return true;
				}
			}

			while (++_current1 < _bits1Length)
			{
				if (_rentedBits.Bits1[_current1] != 0UL)
				{
					_offset1 = _current1 << 6;
					_bits1 = _rentedBits.Bits1[_current1];
					_index1 = MathUtils.LSB(_bits1);
					_runEnd1 = MathUtils.ApproximateMSB(_bits1);

					var setBitCount = MathUtils.PopCount(_bits1);
					var runLength = _runEnd1 - _index1;

					if ((setBitCount << 1) + setBitCount > runLength)
					{
						_bits1 = 0UL;
					}
					else
					{
						_runEnd1 = 0;
					}

					while (_index1 < _runEnd1)
					{
						if ((_rentedBits.Bits1[_current1] & (1UL << _index1)) != 0UL)
						{
							_current0 = _offset1 + _index1;
							_offset0 = _current0 << 6;
							SetupAndRunInnerLoop();
							return true;
						}
						_index1++;
					}

					_current0 = _offset1 + _index1;
					_bits1 &= _bits1 - 1UL;
					_offset0 = _current0 << 6;
					SetupAndRunInnerLoop();
					return true;
				}
			}

			return false;
		}

		public void Dispose()
		{
			BitsPool.ReturnAndPop(_rentedBits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetupAndRunInnerLoop()
		{
			_bits0 = _rentedBits.Bits0[_current0];
			_index0 = MathUtils.LSB(_bits0);
			_runEnd0 = MathUtils.ApproximateMSB(_bits0);

			var setBitCount = MathUtils.PopCount(_bits0);
			var runLength = _runEnd0 - _index0;

			if ((setBitCount << 1) + setBitCount > runLength)
			{
				_bits0 = 0UL;
			}
			else
			{
				_runEnd0 = 0;
			}

			while (_index0 < _runEnd0)
			{
				if ((_rentedBits.Bits0[_current0] & (1UL << _index0)) != 0UL)
				{
					Current = _offset0 + _index0;
					return;
				}
				_index0++;
			}

			Current = _offset0 + _index0;
			_bits0 &= _bits0 - 1UL;
		}
	}
}
