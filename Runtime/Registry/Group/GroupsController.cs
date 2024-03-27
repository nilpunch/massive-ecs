using System;
using System.Collections.Generic;

namespace Massive
{
	public class GroupsController : IGroupsController
	{
		private readonly int _nonOwningDataCapacity;
		private readonly Dictionary<ISet, IGroup> _ownedSets = new Dictionary<ISet, IGroup>();
		private readonly Dictionary<int, IGroup> _groupsLookup = new Dictionary<int, IGroup>();

		protected List<IGroup> CreatedGroups { get; } = new List<IGroup>();

		public GroupsController(int nonOwningDataCapacity = Constants.DataCapacity)
		{
			_nonOwningDataCapacity = nonOwningDataCapacity;
		}

		public IGroup EnsureGroup(ISet[] owned = null, IReadOnlySet[] other = null, IFilter filter = null)
		{
			owned ??= Array.Empty<ISet>();
			other ??= Array.Empty<IReadOnlySet>();
			filter ??= EmptyFilter.Instance;

			int ownedCode = owned.GetUnorderedHashCode();
			int otherCode = other.GetUnorderedHashCode();
			int filterCode = GetFilterHash(filter);
			int groupCode = CombineHashOrdered(CombineHashOrdered(ownedCode, otherCode), filterCode);

			if (!_groupsLookup.TryGetValue(groupCode, out var group))
			{
				if (owned.Length == 0)
				{
					group = CreateNonOwningGroup(other, filter, _nonOwningDataCapacity);
				}
				else
				{
					ThrowIfGroupsConflicting(owned);
					group = CreateOwningGroup(owned, other, filter);
					for (int i = 0; i < owned.Length; i++)
					{
						_ownedSets.Add(owned[i], group);
					}
				}

				_groupsLookup.Add(groupCode, group);
				CreatedGroups.Add(group);
			}

			group.EnsureSynced();

			return group;
		}

		protected virtual IGroup CreateOwningGroup(ISet[] owned, IReadOnlySet[] other = null, IFilter filter = null)
		{
			return new OwningGroup(owned, other, filter);
		}

		protected virtual IGroup CreateNonOwningGroup(IReadOnlySet[] other, IFilter filter = null, int dataCapacity = Constants.DataCapacity)
		{
			return new NonOwningGroup(other, filter, dataCapacity);
		}

		private IGroup FindSubset(ISet[] owned = null, IReadOnlySet[] other = null, IFilter filter = null)
		{
			foreach (var group in CreatedGroups)
			{
				if (group.Filter.IsSubsetOf(filter) && group.Owned.IsSubsetOf(owned) && group.Other.IsSubsetOf(other))
				{
					return group;
				}
			}

			return null;
		}

		private IGroup FindSuperset(ISet[] owned = null, IReadOnlySet[] other = null, IFilter filter = null)
		{
			foreach (var group in CreatedGroups)
			{
				if (filter.IsSubsetOf(group.Filter) && owned.IsSubsetOf(group.Owned) && other.IsSubsetOf(group.Other))
				{
					return group;
				}
			}

			return null;
		}

		private void ThrowIfGroupsConflicting(ISet[] owned)
		{
			for (int i = 0; i < owned.Length; i++)
			{
				if (_ownedSets.ContainsKey(owned[i]))
				{
					throw new Exception("Set is already owned by another group.");
				}
			}
		}

		private static int GetFilterHash(IFilter filter)
		{
			int include = filter.Include.GetUnorderedHashCode();
			int exclude = filter.Exclude.GetUnorderedHashCode();
			return CombineHashOrdered(include, exclude);
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