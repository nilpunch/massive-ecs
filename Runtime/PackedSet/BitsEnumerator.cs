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

		private int _current1;
		private int _iterated1;
		private int _runEnd1;
		private int _offset1;

		private int _current0;
		private int _iterated0;
		private int _runEnd0;
		private int _offset0;

		public BitsEnumerator(Bits rentedBits, Pops rentedPops, int bits1Length)
		{
			_rentedBits = rentedBits;
			_rentedPops = rentedPops;
			_bits1Length = bits1Length;

			_current1 = 0;
			_iterated1 = 64;
			_runEnd1 = 0;
			_offset1 = 0;

			_current0 = 0;
			_iterated0 = 64;
			_runEnd0 = 0;
			_offset0 = 0;

			Current = default;

			WarmupIteration();
		}

		public int Current { get; private set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			while (true)
			{
				Bits0Loop:
				do
				{
					if (_iterated0 < _runEnd0)
					{
						if ((_rentedBits.Bits0[_current0] & (1UL << _iterated0)) != 0)
						{
							Current = _offset0 + _iterated0;
							++_iterated0;
							return true;
						}

						++_iterated0;
						continue;
					}

					if (_iterated0 < 64 && _rentedBits.Bits0[_current0] != 0UL)
					{
						var bits0Result = _rentedBits.Bits0[_current0] >> _iterated0;
						var skip0 = MathUtils.LSB(bits0Result);
						_iterated0 += skip0;
						bits0Result >>= skip0;

						if (bits0Result == 0UL)
						{
							break;
						}

						var runLength0 = bits0Result == ulong.MaxValue ? 64 : MathUtils.LSB(~bits0Result);
						_runEnd0 = _iterated0 + runLength0;
						continue;
					}

					break;
				} while (true);

				do
				{
					if (_iterated1 < _runEnd1)
					{
						if ((_rentedBits.Bits1[_current1] & (1UL << _iterated1)) != 0)
						{
							_current0 = _offset1 + _iterated1;
							_offset0 = _current0 << 6;
							_iterated0 = 0;
							_runEnd0 = 0;
							_iterated1++;
							goto Bits0Loop;
						}

						_iterated1++;
						continue;
					}

					if (_iterated1 < 64 && _rentedBits.Bits1[_current1] != 0UL)
					{
						var bits1Result = _rentedBits.Bits1[_current1] >> _iterated1;
						var skip1 = MathUtils.LSB(bits1Result);
						_iterated1 += skip1;
						bits1Result >>= skip1;

						if (bits1Result == 0UL)
						{
							break;
						}

						var runLength1 = bits1Result == ulong.MaxValue ? 64 : MathUtils.LSB(~bits1Result);
						_runEnd1 = _iterated1 + runLength1;
						continue;
					}

					break;
				} while (true);

				if (++_current1 >= _bits1Length)
				{
					return false;
				}

				_iterated1 = 0;
				_runEnd1 = 0;
				_offset1 = _current1 << 6;
			}
		}

		public void Dispose()
		{
			BitsPool.Return(_rentedBits);
			PopsPool.ReturnAndPop(_rentedPops);
		}

		private void WarmupIteration()
		{
			for (_current1 = 0; _current1 < _bits1Length; _current1++)
			{
				_offset1 = _current1 << 6;
				_iterated1 = 0;

				while (_rentedBits.Bits1[_current1] != 0UL && _iterated1 < 64)
				{
					var bits1Result = _rentedBits.Bits1[_current1] >> _iterated1;

					var skip1 = MathUtils.LSB(bits1Result);
					_iterated1 += skip1;
					bits1Result >>= skip1;

					if (bits1Result == 0UL)
					{
						break;
					}

					var runLength1 = bits1Result == ulong.MaxValue ? 64 : MathUtils.LSB(~bits1Result);
					_runEnd1 = _iterated1 + runLength1;
					for (; _iterated1 < _runEnd1; _iterated1++)
					{
						if ((_rentedBits.Bits1[_current1] & (1UL << _iterated1)) == 0)
						{
							continue;
						}

						_current0 = _offset1 + _iterated1;

						_offset0 = _current0 << 6;
						_iterated0 = 0;

						while (_rentedBits.Bits0[_current0] != 0UL && _iterated0 < 64)
						{
							var bits0Result = _rentedBits.Bits0[_current0] >> _iterated0;

							var skip0 = MathUtils.LSB(bits0Result);
							_iterated0 += skip0;
							bits0Result >>= skip0;

							if (bits0Result == 0UL)
							{
								break;
							}

							var runLength0 = bits0Result == ulong.MaxValue ? 64 : MathUtils.LSB(~bits0Result);
							_runEnd0 = _iterated0 + runLength0;

							_iterated1++;
							return;
						}
					}
				}
			}
		}
	}
}
