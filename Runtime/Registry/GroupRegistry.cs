using System;
using System.Collections.Generic;

namespace Massive
{
	public class GroupRegistry
	{
		private readonly GenericLookup<IGroup> _groupLookup = new GenericLookup<IGroup>();
		private readonly IGroupFactory _groupFactory;
		private readonly Dictionary<ISet, IOwningGroup> _ownedBase = new Dictionary<ISet, IOwningGroup>();

		public GroupRegistry(int nonOwningSetCapacity = Constants.DefaultSetCapacity)
			: this(new NormalGroupFactory(nonOwningSetCapacity))
		{
		}

		protected GroupRegistry(IGroupFactory groupFactory)
		{
			_groupFactory = groupFactory;
		}

		public IReadOnlyList<IGroup> All => _groupLookup.All;

		public IGroup Get<TOwn, TInclude, TExclude>(SetRegistry setRegistry)
			where TOwn : IOwnSelector, new()
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			var group = _groupLookup.GetOrDefault<Tuple<TOwn, TInclude, TExclude>>();

			if (group != null)
			{
				group.EnsureSynced();
				return group;
			}

			var owned = new TOwn().Select(setRegistry);
			var include = new TInclude().SelectReadOnly(setRegistry);
			var exclude = new TExclude().SelectReadOnly(setRegistry);

			// If non-owning, then just create new one
			if (owned.Length == 0)
			{
				var nonOwningGroup = _groupFactory.CreateNonOwningGroup(include, exclude);
				return RegisterAndSync<Tuple<TOwn, TInclude, TExclude>>(nonOwningGroup);
			}

			// Find base group for any owned set
			IOwningGroup baseGroup = null;
			foreach (var ownedSet in owned)
			{
				if (_ownedBase.TryGetValue(ownedSet, out baseGroup))
				{
					break;
				}
			}

			// If there is no base group, just create new owning group
			if (baseGroup == null)
			{
				var owningGroup = _groupFactory.CreateOwningGroup(owned, include, exclude);
				foreach (var set in owned)
				{
					_ownedBase.Add(set, owningGroup);
				}
				return RegisterAndSync<Tuple<TOwn, TInclude, TExclude>>(owningGroup);
			}

			// Try to create new group as extension to the base group
			if (baseGroup.BaseForGroup(owned, include, exclude))
			{
				var baseGroupNode = baseGroup;

				// Find most nested group that is base for our
				while (baseGroupNode.Extended != null && baseGroupNode.Extended.BaseForGroup(owned, include, exclude))
				{
					baseGroupNode = baseGroupNode.Extended;
				}

				// Check if the next group can extend ours
				if (baseGroupNode.Extended != null && !baseGroupNode.Extended.ExtendsGroup(owned, include, exclude))
				{
					throw new Exception("Conflicting groups.");
				}

				var owningGroup = _groupFactory.CreateOwningGroup(owned, include, exclude);
				baseGroupNode.AddGroupAfterThis(owningGroup);
				return RegisterAndSync<Tuple<TOwn, TInclude, TExclude>>(owningGroup);
			}

			// Try to create group as a new base group
			if (baseGroup.ExtendsGroup(owned, include, exclude))
			{
				var owningGroup = _groupFactory.CreateOwningGroup(owned, include, exclude);
				baseGroup.AddGroupBeforeThis(owningGroup);
				foreach (var set in owned)
				{
					_ownedBase[set] = owningGroup;
				}
				return RegisterAndSync<Tuple<TOwn, TInclude, TExclude>>(owningGroup);
			}

			throw new Exception("Conflicting groups.");
		}

		private IGroup RegisterAndSync<TGroupKey>(IGroup group)
		{
			_groupLookup.Assign<TGroupKey>(group);
			group.EnsureSynced();
			return group;
		}
	}
}
