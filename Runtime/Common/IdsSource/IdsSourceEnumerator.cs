using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public struct IdsSourceEnumerator : IDisposable
	{
		private readonly IdsSource _idsSource;
		private readonly PackingMode _originalPackingMode;
		private int _index;
		private int _current;

		public IdsSourceEnumerator(IdsSource idsSource)
		{
			_idsSource = idsSource;
			_originalPackingMode = _idsSource.PackingMode;
			_idsSource.PackingMode = PackingMode.WithHoles;
			_index = _idsSource.Count;
			_current = Constants.InvalidId;
		}

		public int Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _current;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (--_index > _idsSource.Count)
			{
				_index = _idsSource.Count - 1;
			}

			while (_index >= 0 && (_current = _idsSource.Ids[_index]) < 0)
			{
				--_index;
			}

			return _index >= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			_idsSource.PackingMode = _originalPackingMode;
		}
	}
}
