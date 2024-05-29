using System;
using System.Buffers;
using System.Collections.Generic;

namespace Massive
{
	public class GroupRegistry : TypeRegistry<IGroup>
	{
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

		public IGroup Get<TGroupSelector>(TGroupSelector groupSelector) where TGroupSelector : IGroupSelector
		{
			var group = GetOrNull<TGroupSelector>();

			// Try get existing
			if (group != null)
			{
				return group;
			}

			var owned = new ArraySegment<ISet>(ArrayPool<ISet>.Shared.Rent(groupSelector.OwnedCount), 0, groupSelector.OwnedCount);
			var include = new ArraySegment<IReadOnlySet>(ArrayPool<IReadOnlySet>.Shared.Rent(groupSelector.IncludeCount), 0, groupSelector.IncludeCount);
			var exclude = new ArraySegment<IReadOnlySet>(ArrayPool<IReadOnlySet>.Shared.Rent(groupSelector.ExcludeCount), 0, groupSelector.ExcludeCount);

			void ReturnArrays()
			{
				ArrayPool<ISet>.Shared.Return(owned.Array);
				ArrayPool<IReadOnlySet>.Shared.Return(include.Array);
				ArrayPool<IReadOnlySet>.Shared.Return(exclude.Array);
			}

			groupSelector.Select(owned, include, exclude);

			// If non-owning, then just create new one
			if (groupSelector.OwnedCount == 0)
			{
				var nonOwningGroup = _groupFactory.CreateNonOwningGroup(include, exclude);
				ReturnArrays();
				return RegisterAndSync<TGroupSelector>(nonOwningGroup);
			}

			// Find base group for owned sets
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
				ReturnArrays();
				return RegisterAndSync<TGroupSelector>(owningGroup);
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
					ReturnArrays();
					throw new Exception("Conflicting groups.");
				}

				var owningGroup = _groupFactory.CreateOwningGroup(owned, include, exclude);
				baseGroupNode.AddGroupAfterThis(owningGroup);
				ReturnArrays();
				return RegisterAndSync<TGroupSelector>(owningGroup);
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
				ReturnArrays();
				return RegisterAndSync<TGroupSelector>(owningGroup);
			}

			ReturnArrays();
			throw new Exception("Conflicting groups.");
		}

		private IGroup RegisterAndSync<TGroupSelector>(IGroup group)
		{
			Bind<TGroupSelector>(group);
			group.EnsureSynced();
			return group;
		}
	}
}
