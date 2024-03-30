using System;
using System.Collections.Generic;

namespace Massive
{
	public class GroupsController : IGroupsController
	{
		private readonly int _nonOwningDataCapacity;
		private readonly Dictionary<ISet, LinkedList<IGroup>> _ownedBase = new Dictionary<ISet, LinkedList<IGroup>>();
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
			else if (!_ownedBase.TryGetValue(owned[0], out var owningGroups))
			{
				group = CreateOwningGroup(owned, include, exclude);
				owningGroups = new LinkedList<IGroup>(new[] { group });
				foreach (var set in owned)
				{
					_ownedBase.Add(set, owningGroups);
				}
			}
			// Try to create new group as nested
			else if (owningGroups.First.Value.BaseForGroup(owned, include, exclude))
			{
				var owningList = _ownedBase[owned[0]];
				var baseGroupNode = owningList.First;
				
				// Find most nested group that is base for our
				while (baseGroupNode.Next != null && baseGroupNode.Next.Value.BaseForGroup(owned, include, exclude))
				{
					baseGroupNode = baseGroupNode.Next;
				}
				
				// Check if the next group extends ours
				if (baseGroupNode.Next != null && !baseGroupNode.Next.Value.ExtendsGroup(owned, include, exclude))
				{
					throw new Exception("Conflicting groups.");
				}
				
				group = CreateOwningGroup(owned, include, exclude);
				owningList.AddAfter(baseGroupNode, group);
			}
			// Try to create group as base one
			else if (owningGroups.First.Value.ExtendsGroup(owned, include, exclude))
			{
				group = CreateOwningGroup(owned, include, exclude);
				owningGroups.AddBefore(owningGroups.First, group);
				
				foreach (var set in owned)
				{
					_ownedBase[set] = owningGroups;
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

		private bool IsConflicting(ISet[] owned)
		{
			foreach (var set in owned)
			{
				if (_ownedBase.ContainsKey(set))
				{
					return true;
				}
			}

			return false;
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