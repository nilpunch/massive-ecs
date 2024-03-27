using System;
using System.Collections.Generic;

namespace Massive
{
	public class GroupsController : IGroupsController
	{
		private readonly int _nonOwningDataCapacity;
		private readonly HashSet<ISet> _ownedSets = new HashSet<ISet>();
		private readonly Dictionary<int, IGroup> _groupsLookup = new Dictionary<int, IGroup>();

		protected List<IGroup> CreatedGroups { get; } = new List<IGroup>();

		public GroupsController(int nonOwningDataCapacity = Constants.DataCapacity)
		{
			_nonOwningDataCapacity = nonOwningDataCapacity;
		}

		public IGroup EnsureGroup(ISet[] owned = null, ISet[] other = null, IFilter filter = null)
		{
			owned ??= Array.Empty<ISet>();
			other ??= Array.Empty<ISet>();
			filter ??= EmptyFilter.Instance;

			int ownedCode = GetUnorderedHash(owned);
			int otherCode = GetUnorderedHash(other);
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
					for (int i = 0; i < owned.Length; i++)
					{
						_ownedSets.Add(owned[i]);
					}
					group = CreateOwningGroup(owned, other, filter);
				}

				_groupsLookup.Add(groupCode, group);
				CreatedGroups.Add(group);
			}

			group.EnsureSynced();

			return group;
		}

		protected virtual IGroup CreateOwningGroup(ISet[] owned, ISet[] other = null, IFilter filter = null)
		{
			return new OwningGroup(owned, other, filter);
		}

		protected virtual IGroup CreateNonOwningGroup(ISet[] other, IFilter filter = null, int dataCapacity = Constants.DataCapacity)
		{
			return new NonOwningGroup(other, filter, dataCapacity);
		}

		private void ThrowIfGroupsConflicting(ISet[] owned)
		{
			for (int i = 0; i < owned.Length; i++)
			{
				if (_ownedSets.Contains(owned[i]))
				{
					throw new Exception("Set is already owned by another group.");
				}
			}
		}

		private static int GetFilterHash(IFilter filter)
		{
			int include = GetUnorderedHash(filter.Include);
			int exclude = GetUnorderedHash(filter.Exclude);
			return CombineHashOrdered(include, exclude);
		}

		private static int GetUnorderedHash(ISet[] sets)
		{
			int hash = 0;
			for (var i = 0; i < sets.Length; i++)
			{
				hash ^= sets[i].GetHashCode();
			}
			return hash;
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