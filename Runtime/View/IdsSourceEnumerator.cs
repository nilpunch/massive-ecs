using System.Runtime.CompilerServices;

namespace Massive
{
	public struct IdsSourceEnumerator
	{
		private readonly IdsSource _idsSource;
		private int _index;

		public IdsSourceEnumerator(IdsSource idsSource)
		{
			_idsSource = idsSource;
			_index = idsSource.Count;
		}

		public int Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _idsSource.Ids[_index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (--_index > _idsSource.Count)
			{
				_index = _idsSource.Count - 1;
			}

			return _index >= 0;
		}
	}
}
