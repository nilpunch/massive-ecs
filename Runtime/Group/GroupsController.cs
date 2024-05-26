using System;
using System.Collections.Generic;

namespace Massive
{
	public class GroupsController : IGroupsController
	{
		private readonly List<ISet> _ownedBuffer = new List<ISet>();
		private readonly List<IReadOnlySet> _includeBuffer = new List<IReadOnlySet>();
		private readonly List<IReadOnlySet> _excludeBuffer = new List<IReadOnlySet>();

		private readonly IGroupFactory _groupFactory;
		private readonly Dictionary<ISet, IOwningGroup> _ownedBase = new Dictionary<ISet, IOwningGroup>();
		private readonly Dictionary<int, IGroup> _groupsLookup = new Dictionary<int, IGroup>();

		protected List<IGroup> AllGroups { get; } = new List<IGroup>();

		public GroupsController(int nonOwningSetCapacity = Constants.DefaultSetCapacity)
			: this(new NormalGroupFactory(nonOwningSetCapacity))
		{
		}

		protected GroupsController(IGroupFactory groupFactory)
		{
			_groupFactory = groupFactory;
		}

		public IGroup EnsureGroup<TGroupSelector>(TGroupSelector groupSelector) where TGroupSelector : IGroupSelector
		{
			groupSelector.Select(_ownedBuffer, _includeBuffer, _excludeBuffer);

			int ownedCode = _ownedBuffer.GetUnorderedHashCode();
			int includeCode = _includeBuffer.GetUnorderedHashCode();
			int excludeCode = _excludeBuffer.GetUnorderedHashCode();
			int groupCode = MathHelpers.CombineHashes(MathHelpers.CombineHashes(ownedCode, includeCode), excludeCode);

			// Try get existing
			if (_groupsLookup.TryGetValue(groupCode, out var group))
			{
				group.EnsureSynced();
				return group;
			}

			// If non-owning, then just create new one
			if (_ownedBuffer.Count == 0)
			{
				var nonOwningGroup = _groupFactory.CreateNonOwningGroup(_includeBuffer, _excludeBuffer);
				return RegisterAndSync(nonOwningGroup, groupCode);
			}

			// If there is no conflicts, just create new owning group
			if (!_ownedBase.TryGetValue(_ownedBuffer[0], out var baseGroup))
			{
				var owningGroup = _groupFactory.CreateOwningGroup(_ownedBuffer, _includeBuffer, _excludeBuffer);
				foreach (var set in _ownedBuffer)
				{
					_ownedBase.Add(set, owningGroup);
				}
				return RegisterAndSync(owningGroup, groupCode);
			}

			// Try to create new group as extended
			if (baseGroup.BaseForGroup(_ownedBuffer, _includeBuffer, _excludeBuffer))
			{
				var baseGroupNode = baseGroup;

				// Find most nested group that is base for our
				while (baseGroupNode.Extended != null && baseGroupNode.Extended.BaseForGroup(_ownedBuffer, _includeBuffer, _excludeBuffer))
				{
					baseGroupNode = baseGroupNode.Extended;
				}

				// Check if the next group can extend ours
				if (baseGroupNode.Extended != null && !baseGroupNode.Extended.ExtendsGroup(_ownedBuffer, _includeBuffer, _excludeBuffer))
				{
					throw new Exception("Conflicting groups.");
				}

				var owningGroup = _groupFactory.CreateOwningGroup(_ownedBuffer, _includeBuffer, _excludeBuffer);
				baseGroupNode.AddGroupAfterThis(owningGroup);
				return RegisterAndSync(owningGroup, groupCode);
			}

			// Try to create group as base
			if (baseGroup.ExtendsGroup(_ownedBuffer, _includeBuffer, _excludeBuffer))
			{
				var owningGroup = _groupFactory.CreateOwningGroup(_ownedBuffer, _includeBuffer, _excludeBuffer);
				baseGroup.AddGroupBeforeThis(owningGroup);
				foreach (var set in _ownedBuffer)
				{
					_ownedBase[set] = owningGroup;
				}
				return RegisterAndSync(owningGroup, groupCode);
			}

			throw new Exception("Conflicting groups.");
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
