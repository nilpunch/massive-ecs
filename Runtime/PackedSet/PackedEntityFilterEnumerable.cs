﻿using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct PackedMaskedEntityEnumerable
	{
		private readonly PackedSet _packedSet;
		private readonly Mask _rentedMask;
		private readonly World _world;
		private readonly Packing _packingWhenIterating;

		public PackedMaskedEntityEnumerable(PackedSet packedSet, Mask rentedMask,
			World world, Packing packingWhenIterating = Packing.WithHoles)
		{
			_packedSet = packedSet;
			_rentedMask = rentedMask;
			_world = world;
			_packingWhenIterating = packingWhenIterating;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(_packedSet, _rentedMask, _world, _packingWhenIterating);
		}

		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
		public struct Enumerator : IDisposable
		{
			private readonly PackedSet _packedSet;
			private readonly Mask _rentedMask;
			private readonly Entifiers _entifiers;
			private readonly World _world;
			private readonly Packing _originalPacking;
			private int _index;

			public Enumerator(PackedSet packedSet, Mask rentedMask,
				World world, Packing packingWhenIterating = Packing.WithHoles)
			{
				_packedSet = packedSet;
				_rentedMask = rentedMask;
				_entifiers = world.Entifiers;
				_world = world;
				_originalPacking = _packedSet.ExchangeToStricterPacking(packingWhenIterating);
				_index = _packedSet.Count;
			}

			public Entity Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					var id = _packedSet.Packed[_index];
					return new Entity(id, _entifiers.Versions[id], _world);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (--_index > _packedSet.Count)
				{
					_index = _packedSet.Count - 1;
				}

				while (_index >= 0 && !_rentedMask.ContainsId(_packedSet.Packed[_index]))
				{
					--_index;
				}

				return _index >= 0;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose()
			{
				_packedSet.ExchangePacking(_originalPacking);
				_rentedMask.ReturnToPool();
			}
		}
	}
}
