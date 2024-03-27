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

		public IGroup EnsureGroup(IReadOnlyList<ISet> owned = null, IReadOnlyList<IReadOnlySet> other = null, IFilter filter = null)
		{
			owned ??= Array.Empty<ISet>();
			other ??= Array.Empty<IReadOnlySet>();
			filter ??= EmptyFilter.Instance;

			int ownedCode = GetUnorderedHash(owned);
			int otherCode = GetUnorderedHash(other);
			int filterCode = GetFilterHash(filter);
			int groupCode = CombineHashOrdered(CombineHashOrdered(ownedCode, otherCode), filterCode);

			if (!_groupsLookup.TryGetValue(groupCode, out var group))
			{
				if (owned.Count == 0)
				{
					group = CreateNonOwningGroup(other, filter, _nonOwningDataCapacity);
				}
				else
				{
					ThrowIfGroupsConflicting(owned);
					group = CreateOwningGroup(owned, other, filter);
					for (int i = 0; i < owned.Count; i++)
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

		protected virtual IGroup CreateOwningGroup(IReadOnlyList<ISet> owned, IReadOnlyList<IReadOnlySet> other = null, IFilter filter = null)
		{
			return new OwningGroup(owned, other, filter);
		}

		protected virtual IGroup CreateNonOwningGroup(IReadOnlyList<IReadOnlySet> other, IFilter filter = null, int dataCapacity = Constants.DataCapacity)
		{
			return new NonOwningGroup(other, filter, dataCapacity);
		}

		private void ThrowIfGroupsConflicting(IReadOnlyList<ISet> owned)
		{
			for (int i = 0; i < owned.Count; i++)
			{
				if (_ownedSets.ContainsKey(owned[i]))
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

		private static int GetUnorderedHash(IReadOnlyList<IReadOnlySet> sets)
		{
			int hash = 0;
			for (var i = 0; i < sets.Count; i++)
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