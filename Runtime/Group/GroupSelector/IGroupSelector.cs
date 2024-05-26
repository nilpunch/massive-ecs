using System.Collections.Generic;

namespace Massive
{
	public interface IGroupSelector
	{
		void Select(List<ISet> owned, List<IReadOnlySet> include, List<IReadOnlySet> exclude);
	}

	public readonly struct GroupSelector<TOwned, TInclude, TExclude> : IGroupSelector where TOwned : struct, ISetSelector
		where TInclude : struct, IReadOnlySetSelector
		where TExclude : struct, IReadOnlySetSelector
	{
		private readonly IRegistry _registry;

		public GroupSelector(IRegistry registry)
		{
			_registry = registry;
		}

		public void Select(List<ISet> owned, List<IReadOnlySet> include, List<IReadOnlySet> exclude)
		{
			new TOwned().Select(_registry, owned);
			new TInclude().Select(_registry, include);
			new TExclude().Select(_registry, exclude);
		}
	}
}
