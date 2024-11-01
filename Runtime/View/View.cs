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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			foreach (var id in this)
			{
				if (!action.Apply(id))
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
