using System;
using System.Collections.Generic;
using System.Linq;

namespace Massive
{
	public class GroupsController : IGroupsController
	{
		private readonly int _nonOwningDataCapacity;
		private readonly Dictionary<ISet, IGroup> _ownedSupersets = new Dictionary<ISet, IGroup>();
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

			// Try get existing
			if (_groupsLookup.TryGetValue(groupCode, out var group))
			{
				group.EnsureSynced();
				return group;
			}
			
			// If non-owning, then just create new one
			if (owned.Length == 0)
			{
				group = CreateNonOwningGroup(other, filter, _nonOwningDataCapacity);
				group.EnsureSynced();
				_groupsLookup.Add(groupCode, group);
				CreatedGroups.Add(group);
				return group;
			}

			if (!IsConflicting(owned))
			{
				group = CreateOwningGroup(owned, other, filter);
				for (int i = 0; i < owned.Length; i++)
				{
					_ownedSupersets.Add(owned[i], group);
				}
				_groupsLookup.Add(groupCode, group);
				CreatedGroups.Add(group);
	
				group.EnsureSynced();

				return group;
			}

			throw new NotImplementedException();
			
			// Everything beneath this line is WIP
			
			var superset = _ownedSupersets[owned[0]];

			if (superset.BaseForGroup(owned, other, filter))
			{
				while (superset.ExtendedGroup != null && superset.ExtendedGroup.BaseForGroup(owned, other, filter))
				{
					superset = superset.ExtendedGroup;
				}

				if (superset.ExtendedGroup != null)
				{
					if (superset.ExtendedGroup.ExtendsGroup(owned, other, filter))
					{
						group = CreateOwningGroup(owned, other, filter);

						var supersetExtendedGroup = superset.ExtendedGroup;
						supersetExtendedGroup.ExtendedGroup = group;
						group.ExtendedGroup = supersetExtendedGroup;
					}
					else
					{
						throw new Exception("Conflicting groups.");
					}
				}
				else
				{
					group = CreateOwningGroup(owned, other, filter);
					superset.ExtendedGroup = group;
				}
				
				_groupsLookup.Add(groupCode, group);
				CreatedGroups.Add(group);
				
				group.EnsureSynced();
				
				return group;
			}

			if (superset.ExtendsGroup(owned, other, filter))
			{
				group = CreateOwningGroup(owned, other, filter);
				group.ExtendedGroup = superset;
				_ownedSupersets[owned[0]] = group;
				_groupsLookup.Add(groupCode, group);
				CreatedGroups.Add(group);
				
				group.EnsureSynced();
				
				return group;
			}


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

		private void ThrowIfGroupsConflicting(ISet[] owned)
		{
			if (IsConflicting(owned))
			{
				throw new Exception("Set is already owned by another group.");
			}
		}

		private bool IsConflicting(ISet[] owned)
		{
			for (int i = 0; i < owned.Length; i++)
			{
				if (_ownedSupersets.ContainsKey(owned[i]))
				{
					return true;
				}
			}

			return false;
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