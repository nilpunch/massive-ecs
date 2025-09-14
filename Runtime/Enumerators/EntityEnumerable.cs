using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct EntityEnumerable
	{
		private readonly Bits _rentedBits;
		private readonly World _world;
		private readonly int _bits1Length;

		public EntityEnumerable(Bits rentedBits, World world, int bits1Length)
		{
			_rentedBits = rentedBits;
			_world = world;
			_bits1Length = bits1Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(_rentedBits, _world, _bits1Length);
		}

		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
		public struct Enumerator : IDisposable
		{
			private readonly Bits _rentedBits;
			private readonly World _world;
			private readonly Entifiers _entifiers;

			private readonly int _bits1Length;
			private readonly byte[] _deBruijn;

			private int _current;

			private int _current1;
			private int _offset1;
			private ulong _bits1;

			private int _current0;
			private int _offset0;
			private ulong _bits0;

			public Enumerator(Bits rentedBits, World world, int bits1Length)
			{
				_rentedBits = rentedBits;
				_world = world;
				_entifiers = world.Entifiers;
				_bits1Length = bits1Length;

				_current1 = -1;
				_offset1 = default;
				_bits1 = default;
				_current0 = default;
				_offset0 = default;
				_bits0 = default;

				_deBruijn = MathUtils.DeBruijn;

				_current = default;

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

			public Entity Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => new Entity(_current, _entifiers.Versions[_current], _world);
			}

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
					_current = _offset0 + _deBruijn[(int)(((_bits0 & (ulong)-(long)_bits0) * 0x37E84A99DAE458FUL) >> 58)];
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
					_current = _offset0 + _deBruijn[(int)(((_bits0 & (ulong)-(long)_bits0) * 0x37E84A99DAE458FUL) >> 58)];
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
						_current = _offset0 + _deBruijn[(int)(((_bits0 & (ulong)-(long)_bits0) * 0x37E84A99DAE458FUL) >> 58)];
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
}
