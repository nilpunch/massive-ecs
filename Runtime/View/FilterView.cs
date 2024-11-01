using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public readonly struct FilterView : IView
	{
		private Filter Filter { get; }

		public Registry Registry { get; }

		public FilterView(Registry registry, Filter filter = null)
		{
			Registry = registry;
			Filter = filter ?? Filter.Empty;
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
		public IdsSourceFilterEnumerator GetEnumerator()
		{
			IdsSource idsSource = Filter.Include.Length == 0
				? Registry.Entities
				: SetHelpers.GetMinimalSet(Filter.Include);

			return new IdsSourceFilterEnumerator(idsSource, Filter);
		}
	}
}
