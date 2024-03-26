using System;
using System.Collections.Generic;

namespace Massive
{
	public class GroupsController : IGroupsController
	{
		private readonly int _nonOwningDataCapacity;
		private readonly HashSet<ISet> _ownedSets = new HashSet<ISet>();
		private readonly Dictionary<int, IGroup> _owningGroups = new Dictionary<int, IGroup>();
		private readonly Dictionary<int, IGroup> _nonOwningGroups = new Dictionary<int, IGroup>();

		public List<IGroup> CreatedGroups { get; } = new List<IGroup>();

		public GroupsController(int nonOwningDataCapacity = Constants.DataCapacity)
		{
			_nonOwningDataCapacity = nonOwningDataCapacity;
		}

		public IGroup EnsureOwningGroup(ISet[] owned, ISet[] other = null, IFilter filter = null)
		{
			int ownedCode = CombineHashCodeUnordered(owned);
			int otherCode = CombineHashCodeUnordered(other ?? Array.Empty<ISet>());
			int filterCode = FilterHashCode(filter ?? EmptyFilter.Instance);

			int groupCode = CombineHashCodeOrdered(CombineHashCodeOrdered(ownedCode, otherCode), filterCode);

			if (_owningGroups.TryGetValue(groupCode, out var group))
			{
				return group;
			}

			for (int i = 0; i < owned.Length; i++)
			{
				if (_ownedSets.Contains(owned[i]))
				{
					throw new Exception("Conflicting groups.");
				}
			}

			var newGroup = CreateOwningGroup(owned, other, filter);
			for (int i = 0; i < owned.Length; i++)
			{
				_ownedSets.Add(owned[i]);
			}
			_owningGroups.Add(groupCode, newGroup);
			CreatedGroups.Add(newGroup);
			return newGroup;
		}

		public IGroup EnsureNonOwningGroup(ISet[] other, IFilter filter = null)
		{
			int otherCode = CombineHashCodeUnordered(other ?? Array.Empty<ISet>());
			int filterCode = FilterHashCode(filter ?? EmptyFilter.Instance);

			int groupCode = CombineHashCodeOrdered(otherCode, filterCode);

			if (_nonOwningGroups.TryGetValue(groupCode, out var group))
			{
				return group;
			}

			var newGroup = CreateNonOwningGroup(other, _nonOwningDataCapacity, filter);
			_nonOwningGroups.Add(groupCode, newGroup);
			CreatedGroups.Add(newGroup);
			return newGroup;
		}

		protected virtual IGroup CreateOwningGroup(ISet[] owned, ISet[] other = null, IFilter filter = null)
		{
			return new OwningGroup(owned, other, filter);
		}

		protected virtual IGroup CreateNonOwningGroup(ISet[] other, int dataCapacity, IFilter filter = null)
		{
			return new NonOwningGroup(other, dataCapacity, filter);
		}

		private static int FilterHashCode(IFilter filter)
		{
			int include = CombineHashCodeUnordered(filter.Include);
			int exclude = CombineHashCodeUnordered(filter.Exclude);
			return CombineHashCodeOrdered(include, exclude);
		}

		private static int CombineHashCodeUnordered(ISet[] sets)
		{
			int hash = 0;
			for (var i = 0; i < sets.Length; i++)
			{
				hash ^= sets[i].GetHashCode();
			}
			return hash;
		}

		private static int CombineHashCodeOrdered(int a, int b)
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 31 + a;
				hash = hash * 31 + b;
				return hash;
			}
			;
		}
	}
}