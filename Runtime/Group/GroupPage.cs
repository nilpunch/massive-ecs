using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public readonly struct GroupPage
	{
		public int Index { get; }
		public int Length { get; }
		public int Offset { get; }
		public Group Group { get; }

		public GroupPage(int pageIndex, int pageLength, int indexOffset, Group group)
		{
			Index = pageIndex;
			Length = pageLength;
			Offset = indexOffset;
			Group = group;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(Length, Offset, Group);
		}

		public struct Enumerator
		{
			private readonly Group _group;
			private readonly int _indexOffset;
			private int _index;

			public Enumerator(int pageLength, int indexOffset, Group group)
			{
				_indexOffset = indexOffset;
				_group = group;
				_index = pageLength;
			}

			public int Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _index;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (--_index + _indexOffset > _group.Count)
				{
					_index = _group.Count - _indexOffset - 1;
				}

				return _index >= 0;
			}
		}
	}
}
