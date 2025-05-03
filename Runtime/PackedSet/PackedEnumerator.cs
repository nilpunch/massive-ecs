using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct PackedEnumerator : IDisposable
	{
		private readonly PackedSet _packedSet;
		private readonly Packing _originalPacking;
		private int _index;

		public PackedEnumerator(PackedSet packedSet, Packing packingWhenIterating = Packing.WithHoles)
		{
			_packedSet = packedSet;
			_originalPacking = _packedSet.ExchangeToStricterPacking(packingWhenIterating);
			_index = _packedSet.Count;
		}

		public int Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _packedSet.Packed[_index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (--_index > _packedSet.Count)
			{
				_index = _packedSet.Count - 1;
			}

			while (_index >= 0 && _packedSet.Packed[_index] < 0)
			{
				--_index;
			}

			return _index >= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			_packedSet.ExchangePacking(_originalPacking);
		}
	}
}
