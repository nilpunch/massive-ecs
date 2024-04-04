using System;
using System.Collections.Generic;

namespace Massive
{
	public class GroupsController : IGroupsController
	{
		private readonly int _nonOwningDataCapacity;
		private readonly Dictionary<ISet, IGroup> _ownedBase = new Dictionary<ISet, IGroup>();
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
				group = CreateNonOwningGroup(include, exclude, _nonOwningDataCapacity);
			}
			// Create new owning group if there is no conflicts
			else if (!_ownedBase.TryGetValue(owned[0], out var baseGroup))
			{
				group = CreateOwningGroup(owned, include, exclude);
				foreach (var set in owned)
				{
					_ownedBase.Add(set, group);
				}
			}
			// Try to create new group as nested
			else if (baseGroup.BaseForGroup(owned, include, exclude))
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

				group = CreateOwningGroup(owned, include, exclude);
				baseGroupNode.AddAfterThis(group);
			}
			// Try to create group as base one
			else if (baseGroup.ExtendsGroup(owned, include, exclude))
			{
				group = CreateOwningGroup(owned, include, exclude);
				baseGroup.AddBeforeThis(group);

				foreach (var set in owned)
				{
					_ownedBase[set] = group;
				}
			}
			else
			{
				throw new Exception("Conflicting groups.");
			}

			_groupsLookup.Add(groupCode, group);
			CreatedGroups.Add(group);
			group.EnsureSynced();

			return group;
		}

		protected virtual IGroup CreateOwningGroup(ISet[] owned, IReadOnlySet[] include = null, IReadOnlySet[] exclude = null)
		{
			return new OwningGroup(owned, include, exclude);
		}

		protected virtual IGroup CreateNonOwningGroup(IReadOnlySet[] include, IReadOnlySet[] exclude = null, int dataCapacity = Constants.DataCapacity)
		{
			return new NonOwningGroup(include, exclude, dataCapacity);
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