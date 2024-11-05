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
			private readonly SparseSet _mainSet;
			private readonly int _indexOffset;
			private int _index;

			public Enumerator(int pageLength, int indexOffset, Group group)
			{
				_indexOffset = indexOffset;
				_group = group;
				_mainSet = group.MainSet;
				_index = pageLength;
			}

			public Entry Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Entry.Create(_index, _mainSet.Ids[_index + _indexOffset]);
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

		public readonly struct Entry
		{
			private readonly long _indexAndId;

			/// <summary>
			/// Index in page.
			/// </summary>
			public int Index
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (int)(_indexAndId & 0x00000000FFFFFFFF);
			}

			public int Id
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (int)(_indexAndId >> 32);
			}

			public Entry(long indexAndId)
			{
				_indexAndId = indexAndId;
			}

#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Entry Create(int indexInPage, int id)
			{
				long packed = indexInPage | ((long)id << 32);
				return new Entry(packed);
			}
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
		}
	}
}
