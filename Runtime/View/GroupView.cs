using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public readonly struct GroupView : IView
	{
		private Group Group { get; }

		public Registry Registry { get; }

		public GroupView(Registry registry, Group group)
		{
			Registry = registry;
			Group = group;
		}

		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			Group.EnsureSynced();

			var groupSet = Group.MainSet;
			for (var i = Group.Count - 1; i >= 0; i--)
			{
				if (i > Group.Count)
				{
					i = Group.Count;
					continue;
				}

				if (!action.Apply(groupSet.Ids[i]))
				{
					break;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(Group);
		}

		public struct Enumerator
		{
			private readonly SparseSet _mainSet;
			private readonly Group _group;
			private int _index;

			public Enumerator(Group group)
			{
				_group = group;
				_mainSet = group.MainSet;
				_index = group.Count;
			}

			public int Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _mainSet.Ids[_index];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (--_index > _group.Count)
				{
					_index = _group.Count - 1;
				}

				return _index >= 0;
			}
		}
	}
}
