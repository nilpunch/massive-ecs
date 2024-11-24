using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public struct IdsEnumerator : IDisposable
	{
		private readonly IdsSource _idsSource;
		private readonly Packing _originalPacking;
		private int _index;

		public IdsEnumerator(IdsSource idsSource, Packing packingWhenIterating = Packing.WithHoles)
		{
			_idsSource = idsSource;
			_originalPacking = _idsSource.ExchangeToStricterPacking(packingWhenIterating);
			_index = _idsSource.Count;
		}

		public int Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _idsSource.Ids[_index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			while (--_index >= 0 && (_index >= _idsSource.Count || _idsSource.Ids[_index] < 0))
			{
			}

			return _index >= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			_idsSource.ExchangePacking(_originalPacking);
		}
	}
}
