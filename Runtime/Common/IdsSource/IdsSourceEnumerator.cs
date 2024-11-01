using System.Runtime.CompilerServices;

namespace Massive
{
	public struct IdsSourceEnumerator
	{
		private readonly IdsSource _idsSource;
		private int _index;
		private int _current;

		public IdsSourceEnumerator(IdsSource idsSource)
		{
			_idsSource = idsSource;
			_index = idsSource.Count;
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
	}
}
