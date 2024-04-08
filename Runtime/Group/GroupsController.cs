using System;
using System.Collections.Generic;

namespace Massive
{
	public class GroupsController : IGroupsController
	{
		private readonly int _nonOwningDataCapacity;
		private readonly Dictionary<ISet, IOwningGroup> _ownedBase = new Dictionary<ISet, IOwningGroup>();
		private readonly Dictionary<int, IGroup> _groupsLookup = new Dictionary<int, IGroup>();

		protected List<IGroup> AllGroups { get; } = new List<IGroup>();

		public GroupsController(int nonOwningDataCapacity = Constants.DataCapacity)
		{
			_nonOwningDataCapacity = nonOwningDataCapacity;
		}

		public IGroup EnsureGroup(IReadOnlyList<ISet> owned = null, IReadOnlyList<IReadOnlySet> include = null, IReadOnlyList<IReadOnlySet> exclude = null)
		{
			owned ??= Array.Empty<ISet>();
			include ??= Array.Empty<IReadOnlySet>();
			exclude ??= Array.Empty<IReadOnlySet>();

			int ownedCode = owned.GetUnorderedHashCode();
			int includeCode = include.GetUnorderedHashCode();
			int excludeCode = exclude.GetUnorderedHashCode();
			int groupCode = MathHelpers.CombineHashes(MathHelpers.CombineHashes(ownedCode, includeCode), excludeCode);

			// Try get existing
			if (_groupsLookup.TryGetValue(groupCode, out var group))
			{
				group.EnsureSynced();
				return group;
			}

			// If non-owning, then just create new one
			if (owned.Count == 0)
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

		protected virtual IOwningGroup CreateOwningGroup(IReadOnlyList<ISet> owned,
			IReadOnlyList<IReadOnlySet> include = null, IReadOnlyList<IReadOnlySet> exclude = null)
		{
			return new OwningGroup(owned, include, exclude);
		}

		protected virtual IGroup CreateNonOwningGroup(IReadOnlyList<IReadOnlySet> include,
			IReadOnlyList<IReadOnlySet> exclude = null, int dataCapacity = 100)
		{
			return new NonOwningGroup(include, exclude, dataCapacity);
		}

		private IGroup RegisterAndSync(IGroup group, int groupCode)
		{
			_groupsLookup.Add(groupCode, group);
			AllGroups.Add(group);
			group.EnsureSynced();
			return group;
		}
	}
}