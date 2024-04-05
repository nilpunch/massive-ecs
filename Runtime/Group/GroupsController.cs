using System;
using System.Collections.Generic;

namespace Massive
{
	public class GroupsController : IGroupsController
	{
		private readonly int _nonOwningDataCapacity;
		private readonly Dictionary<ISet, IOwningGroup> _ownedBase = new Dictionary<ISet, IOwningGroup>();
		private readonly Dictionary<int, IGroup> _groupsLookup = new Dictionary<int, IGroup>();

		protected List<IGroup> CreatedGroups { get; } = new List<IGroup>();

		public GroupsController(int nonOwningDataCapacity = Constants.DataCapacity)
		{
			_nonOwningDataCapacity = nonOwningDataCapacity;
		}

		public IGroup EnsureGroup(ISet[] owned = null, IReadOnlySet[] include = null, IReadOnlySet[] exclude = null)
		{
			owned ??= Array.Empty<ISet>();
			include ??= Array.Empty<IReadOnlySet>();
			exclude ??= Array.Empty<IReadOnlySet>();

			int ownedCode = owned.GetUnorderedHashCode();
			int includeCode = include.GetUnorderedHashCode();
			int excludeCode = exclude.GetUnorderedHashCode();
			int groupCode = CombineHashOrdered(CombineHashOrdered(ownedCode, includeCode), excludeCode);

			// Try get existing
			if (_groupsLookup.TryGetValue(groupCode, out var group))
			{
				group.EnsureSynced();
				return group;
			}

			// If non-owning, then just create new one
			if (owned.Length == 0)
			{
				var nonOwningGroup = CreateNonOwningGroup(include, exclude, _nonOwningDataCapacity);
				return RegisterAndSync(nonOwningGroup, groupCode);
			}

			// If there is no conflicts, just create new owning group
			if (!_ownedBase.TryGetValue(owned[0], out var baseGroup))
			{
				var owningGroup = CreateOwningGroup(owned, include, exclude);
				foreach (var set in owned)
				{
					_ownedBase.Add(set, owningGroup);
				}
				return RegisterAndSync(owningGroup, groupCode);
			}

			// Try to create new group as extended
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

				var owningGroup = CreateOwningGroup(owned, include, exclude);
				baseGroupNode.AddGroupAfterThis(owningGroup);
				return RegisterAndSync(owningGroup, groupCode);
			}

			// Try to create group as base
			if (baseGroup.ExtendsGroup(owned, include, exclude))
			{
				var owningGroup = CreateOwningGroup(owned, include, exclude);
				baseGroup.AddGroupBeforeThis(owningGroup);
				foreach (var set in owned)
				{
					_ownedBase[set] = owningGroup;
				}
				return RegisterAndSync(owningGroup, groupCode);
			}

			throw new Exception("Conflicting groups.");
		}

		protected virtual IOwningGroup CreateOwningGroup(ISet[] owned, IReadOnlySet[] include = null, IReadOnlySet[] exclude = null)
		{
			return new OwningGroup(owned, include, exclude);
		}

		protected virtual IGroup CreateNonOwningGroup(IReadOnlySet[] include, IReadOnlySet[] exclude = null, int dataCapacity = 100)
		{
			return new NonOwningGroup(include, exclude, dataCapacity);
		}

		private IGroup RegisterAndSync(IGroup group, int groupCode)
		{
			_groupsLookup.Add(groupCode, group);
			CreatedGroups.Add(group);
			group.EnsureSynced();
			return group;
		}

		private static int CombineHashOrdered(int a, int b)
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 31 + a;
				hash = hash * 31 + b;
				return hash;
			}
		}
	}
}