using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public struct PackedFilterEnumerator : IDisposable
	{
		private readonly PackedSet _packedSet;
		private readonly TrimmedFilter _trimmedFilter;
		private readonly Packing _originalPacking;
		private int _index;

		public PackedFilterEnumerator(PackedSet packedSet, TrimmedFilter trimmedFilter,
			Packing packingWhenIterating = Packing.WithHoles)
		{
			_packedSet = packedSet;
			_trimmedFilter = trimmedFilter;
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
			while (--_index >= 0 && (_index >= _packedSet.Count || !_trimmedFilter.ContainsId(_packedSet.Packed[_index])))
			{
			}

			return _index >= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			_trimmedFilter.Dispose();
			_packedSet.ExchangePacking(_originalPacking);
		}
	}
}
