using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct BitsEnumerator : IDisposable
	{
		private readonly QueryCache _cache;

		private readonly byte[] _deBruijn;

		private readonly int[] _nonEmptyBitsIndices;
		private readonly int _nonEmptyBitsCount;
		private int _nonEmptyBitsIndex;
		private bool _useRange;

		private int _bit;
		private int _runEnd;

		private int _bitsIndex;
		private int _bitsOffset;
		private ulong _bits;

		public BitsEnumerator(QueryCache cache)
		{
			_cache = cache;
			_nonEmptyBitsCount = cache.NonEmptyBitsCount;
			_nonEmptyBitsIndices = cache.NonEmptyBitsIndices;

			_deBruijn = MathUtils.DeBruijn;

			_nonEmptyBitsIndex = -1;
			_bit = default;
			_runEnd = default;
			_bitsIndex = default;
			_bitsOffset = default;
			_bits = default;

			Current = default;

			while (++_nonEmptyBitsIndex < _nonEmptyBitsCount)
			{
				_bitsIndex = _nonEmptyBitsIndices[_nonEmptyBitsIndex];
				if (_cache.Bits[_bitsIndex] != 0UL)
				{
					_bits = _cache.Bits[_bitsIndex];
					_bitsOffset = _bitsIndex << 6;
					_bit = _deBruijn[(int)(((_bits & (ulong)-(long)_bits) * 0x37E84A99DAE458FUL) >> 58)];

					_runEnd = MathUtils.ApproximateMSB(_bits);
					var setBits = MathUtils.PopCount(_bits);

					_useRange = setBits << 1 > _runEnd - _bit;
					return;
				}
			}

			_useRange = true;
		}

		public int Current { get; private set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (_useRange)
			{
				while (_bit < _runEnd)
				{
					if ((_cache.Bits[_bitsIndex] & (1UL << _bit)) == 0UL)
					{
						_bit++;
						continue;
					}

					Current = _bitsOffset + _bit++;
					return true;
				}
			}
			else
			{
				_bits &= _cache.Bits[_bitsIndex];
				if (_bits != 0UL)
				{
					_bit = _deBruijn[(int)(((_bits & (ulong)-(long)_bits) * 0x37E84A99DAE458FUL) >> 58)];
					_bits &= _bits - 1UL;
					Current = _bitsOffset + _bit;
					return true;
				}
			}

			while (++_nonEmptyBitsIndex < _nonEmptyBitsCount)
			{
				_bitsIndex = _nonEmptyBitsIndices[_nonEmptyBitsIndex];
				if (_cache.Bits[_bitsIndex] != 0UL)
				{
					_bits = _cache.Bits[_bitsIndex];
					_bitsOffset = _bitsIndex << 6;
					_bit = _deBruijn[(int)(((_bits & (ulong)-(long)_bits) * 0x37E84A99DAE458FUL) >> 58)];

					_runEnd = MathUtils.ApproximateMSB(_bits);
					var setBits = MathUtils.PopCount(_bits);

					_useRange = setBits << 1 > _runEnd - _bit;

					Current = _bitsOffset + _bit++;
					_bits &= _bits - 1UL;
					return true;
				}
			}

			return false;
		}

		public void Dispose()
		{
			QueryCache.ReturnAndPop(_cache);
		}
	}
}
