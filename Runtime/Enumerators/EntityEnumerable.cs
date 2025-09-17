﻿using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct EntityEnumerable
	{
		private readonly QueryCache _cache;
		private readonly World _world;

		public EntityEnumerable(QueryCache cache, World world)
		{
			_cache = cache;
			_world = world;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(_cache, _world);
		}

		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
		public struct Enumerator : IDisposable
		{
			private readonly QueryCache _cache;
			private readonly World _world;
			private readonly Entifiers _entifiers;

			private readonly int _nonEmptyBitsCount;
			private readonly byte[] _deBruijn;

			private int _current;

			private int _blockIndex;
			private int _blockOffset;
			private ulong _block;

			private int _bitsIndex;
			private int _bitsOffset;
			private ulong _bits;

			public Enumerator(QueryCache cache, World world)
			{
				_cache = cache;
				_world = world;
				_entifiers = world.Entifiers;
				_nonEmptyBitsCount = cache.NonEmptyBitsCount;

				_blockIndex = -1;
				_blockOffset = default;
				_block = default;
				_bitsIndex = default;
				_bitsOffset = default;
				_bits = default;

				_deBruijn = MathUtils.DeBruijn;

				_current = default;

				while (++_blockIndex < _nonEmptyBitsCount)
				{
					if (_cache.NonEmptyBlocks[_blockIndex] != 0UL)
					{
						_blockOffset = _blockIndex << 6;
						_block = _cache.NonEmptyBlocks[_blockIndex];

						_bitsIndex = _blockOffset + _deBruijn[(int)(((_block & (ulong)-(long)_block) * 0x37E84A99DAE458FUL) >> 58)];
						_block &= _block - 1UL;
						_bitsOffset = _bitsIndex << 6;

						_bits = _cache.Bits[_bitsIndex];
						return;
					}
				}

				_nonEmptyBitsCount = 0;
			}

			public Entity Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => new Entity(_current, _entifiers.Versions[_current], _world);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (_nonEmptyBitsCount == 0)
				{
					return false;
				}

				_bits &= _cache.Bits[_bitsIndex];
				if (_bits != 0UL)
				{
					_current = _bitsOffset + _deBruijn[(int)(((_bits & (ulong)-(long)_bits) * 0x37E84A99DAE458FUL) >> 58)];
					_bits &= _bits - 1UL;
					return true;
				}

				_block &= _cache.NonEmptyBlocks[_blockIndex];
				if (_block != 0UL)
				{
					_bitsIndex = _blockOffset + _deBruijn[(int)(((_block & (ulong)-(long)_block) * 0x37E84A99DAE458FUL) >> 58)];
					_block &= _block - 1UL;
					_bitsOffset = _bitsIndex << 6;

					_bits = _cache.Bits[_bitsIndex];
					_current = _bitsOffset + _deBruijn[(int)(((_bits & (ulong)-(long)_bits) * 0x37E84A99DAE458FUL) >> 58)];
					_bits &= _bits - 1UL;
					return true;
				}

				while (++_blockIndex < _nonEmptyBitsCount)
				{
					if (_cache.NonEmptyBlocks[_blockIndex] != 0UL)
					{
						_blockOffset = _blockIndex << 6;
						_block = _cache.NonEmptyBlocks[_blockIndex];

						_bitsIndex = _blockOffset + _deBruijn[(int)(((_block & (ulong)-(long)_block) * 0x37E84A99DAE458FUL) >> 58)];
						_block &= _block - 1UL;
						_bitsOffset = _bitsIndex << 6;

						_bits = _cache.Bits[_bitsIndex];
						_current = _bitsOffset + _deBruijn[(int)(((_bits & (ulong)-(long)_bits) * 0x37E84A99DAE458FUL) >> 58)];
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
}
