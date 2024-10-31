using System.Runtime.CompilerServices;

namespace Massive
{
	public struct IdsSourceFilterEnumerator
	{
		private readonly IdsSource _idsSource;
		private readonly Filter _filter;
		private int _index;
		private int _current;

		public IdsSourceFilterEnumerator(IdsSource idsSource, Filter filter)
		{
			_idsSource = idsSource;
			_filter = filter;
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

			while (_index >= 0 && !_filter.ContainsId(_current = _idsSource.Ids[_index]))
			{
				--_index;
			}

			return _index >= 0;
		}
	}
}
