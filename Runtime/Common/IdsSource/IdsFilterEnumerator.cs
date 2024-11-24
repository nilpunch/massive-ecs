using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public struct IdsFilterEnumerator : IDisposable
	{
		private readonly IdsSource _idsSource;
		private readonly TrimmedFilter _trimmedFilter;
		private readonly Packing _originalPacking;
		private int _index;

		public IdsFilterEnumerator(IdsSource idsSource, TrimmedFilter trimmedFilter,
			Packing packingWhenIterating = Packing.WithHoles)
		{
			_idsSource = idsSource;
			_trimmedFilter = trimmedFilter;
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
			while (--_index >= 0 && (_index >= _idsSource.Count || !_trimmedFilter.ContainsId(_idsSource.Ids[_index])))
			{
			}

			return _index >= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			_trimmedFilter.Dispose();
			_idsSource.ExchangePacking(_originalPacking);
		}
	}
}
