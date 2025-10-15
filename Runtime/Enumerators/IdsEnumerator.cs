using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct IdsEnumerator : IDisposable
	{
		private readonly QueryCache _cache;
		private readonly ulong[] _cachedBits;
		private readonly int[] _nonEmptyBitsIndices;
		private readonly int _nonEmptyBitsCount;

		private readonly byte[] _deBruijn;

		private int _nonEmptyBitsIndex;
		private int _bit;
		private int _runEnd;
		private int _bitsIndex;
		private int _bitsOffset;
		private ulong _bits;

		public IdsEnumerator(QueryCache cache)
		{
			_cache = cache;
			_cachedBits = cache.Bits;
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
				if (_cachedBits[_bitsIndex] != 0UL)
				{
					_bitsOffset = _bitsIndex << 6;
					_bits = _cachedBits[_bitsIndex];
					_bit = _deBruijn[(int)(((_bits & (ulong)-(long)_bits) * 0x37E84A99DAE458FUL) >> 58)];

					_runEnd = MathUtils.ApproximateMSB(_bits);
					var setBits = MathUtils.PopCount(_bits);
					var useRange = setBits << 1 > _runEnd - _bit;

					if (useRange)
					{
						_bits = 0UL;
					}
					else
					{
						_runEnd = 0;
					}

					_bit--;
					return;
				}
			}
		}

		public int Current { get; private set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			while (++_bit < _runEnd)
			{
				if ((_cachedBits[_bitsIndex] & (1UL << _bit)) != 0UL)
				{
					Current = _bitsOffset + _bit;
					return true;
				}
			}

			_bits &= _cachedBits[_bitsIndex];
			if (_bits != 0UL)
			{
				Current = _bitsOffset + _deBruijn[(int)(((_bits & (ulong)-(long)_bits) * 0x37E84A99DAE458FUL) >> 58)];
				_bits &= _bits - 1UL;
				return true;
			}

			while (++_nonEmptyBitsIndex < _nonEmptyBitsCount)
			{
				_bitsIndex = _nonEmptyBitsIndices[_nonEmptyBitsIndex];
				if (_cachedBits[_bitsIndex] != 0UL)
				{
					_bitsOffset = _bitsIndex << 6;
					_bits = _cachedBits[_bitsIndex];
					_bit = _deBruijn[(int)(((_bits & (ulong)-(long)_bits) * 0x37E84A99DAE458FUL) >> 58)];

					_runEnd = MathUtils.ApproximateMSB(_bits);
					var setBits = MathUtils.PopCount(_bits);
					var useRange = setBits << 1 > _runEnd - _bit;

					Current = _bitsOffset + _bit;
					_bits &= _bits - 1UL;

					if (useRange)
					{
						_bits = 0UL;
					}
					else
					{
						_runEnd = 0;
					}

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
