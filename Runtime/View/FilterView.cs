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

		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			IdsSource idsSource = Filter.Include.Length == 0
				? Registry.Entities
				: SetHelpers.GetMinimalSet(Filter.Include);

			for (var i = idsSource.Count - 1; i >= 0; i--)
			{
				if (i > idsSource.Count)
				{
					i = idsSource.Count;
					continue;
				}

				var id = idsSource.Ids[i];
				if (Filter.ContainsId(id))
				{
					if (!action.Apply(id))
					{
						break;
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IdsSourceFilterEnumerator GetEnumerator()
		{
			if (Filter.Include.Length == 0)
			{
				return new IdsSourceFilterEnumerator(Registry.Entities, Filter);
			}
			else
			{
				var ids = SetHelpers.GetMinimalSet(Filter.Include);
				return new IdsSourceFilterEnumerator(ids, Filter);
			}
		}
	}
}
