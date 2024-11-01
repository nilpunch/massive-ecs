using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public readonly struct View : IView
	{
		public Registry Registry { get; }

		public View(Registry registry)
		{
			Registry = registry;
		}

		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			var entities = Registry.Entities;
			for (var i = entities.Count - 1; i >= 0; i--)
			{
				if (i > entities.Count)
				{
					i = entities.Count;
					continue;
				}

				if (!action.Apply(entities.Ids[i]))
				{
					break;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IdsSourceEnumerator GetEnumerator()
		{
			return new IdsSourceEnumerator(Registry.Entities);
		}
	}
}
