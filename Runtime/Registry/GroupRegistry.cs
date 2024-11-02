using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class GroupRegistry
	{
		private readonly GenericLookup<Group> _groupGenericLookup = new GenericLookup<Group>();
		private readonly Dictionary<int, Group> _groupCodeLookup = new Dictionary<int, Group>();
		private readonly FastList<Group> _allGroups = new FastList<Group>();

		private readonly Dictionary<SparseSet, OwningGroup> _ownedBase = new Dictionary<SparseSet, OwningGroup>();
		private readonly Entities _entities;
		private readonly SetRegistry _setRegistry;
		private readonly IGroupFactory _groupFactory;

		public GroupRegistry(SetRegistry setRegistry, IGroupFactory groupFactory, Entities entities)
		{
			_setRegistry = setRegistry;
			_groupFactory = groupFactory;
			_entities = entities;
		}

		public ReadOnlySpan<Group> All
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _allGroups.ReadOnlySpan;
		}

		public Group Get(SparseSet[] include = null, SparseSet[] exclude = null, SparseSet[] owned = null)
		{
			owned ??= Array.Empty<SparseSet>();
			include ??= Array.Empty<SparseSet>();
			exclude ??= Array.Empty<SparseSet>();

			if (ContainsDuplicates(include))
			{
				throw new ArgumentException("Include contains duplicate sets.", nameof(include));
			}
			if (ContainsDuplicates(exclude))
			{
				throw new ArgumentException("Exclude contains duplicate sets.", nameof(exclude));
			}
			if (ContainsDuplicates(owned))
			{
				throw new ArgumentException("Owned contains duplicate sets.", nameof(owned));
			}

			int ownedCode = owned.GetUnorderedHashCode(_setRegistry);
			int includeCode = include.GetUnorderedHashCode(_setRegistry);
			int excludeCode = exclude.GetUnorderedHashCode(_setRegistry);
			int groupCode = MathHelpers.CombineHashes(MathHelpers.CombineHashes(ownedCode, includeCode), excludeCode);

			if (_groupCodeLookup.TryGetValue(groupCode, out var group))
			{
				return group;
			}

			if (Array.Exists(owned, set => set.PackingMode == PackingMode.WithHoles))
			{
				var faultySet = Array.Find(owned, set => set.PackingMode == PackingMode.WithHoles);
				throw new Exception($"Set with direct storage is not supported for owning: {_setRegistry.GetKey(faultySet).GetFullGenericName()}.");
			}

			// If non-owning, then just create new one
			if (owned.Length == 0)
			{
				var entitiesIfNoIncludes = include.Length == 0 ? _entities : null;
				var nonOwningGroup = _groupFactory.CreateNonOwningGroup(include, exclude, entitiesIfNoIncludes);
				return RegisterAndSync(groupCode, nonOwningGroup);
			}

			// Find base group for any owned set
			OwningGroup baseGroup = null;
			foreach (var ownedSet in owned)
			{
				if (_ownedBase.TryGetValue(ownedSet, out baseGroup))
				{
					break;
				}
			}

			// If there is no base group, just create new owning group
			if (baseGroup == null)
			{
				var owningGroup = _groupFactory.CreateOwningGroup(owned, include, exclude);
				foreach (var set in owned)
				{
					_ownedBase.Add(set, owningGroup);
				}
				return RegisterAndSync(groupCode, owningGroup);
			}

			// Try to create new group as extension to the base group
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
					throw new Exception("Conflicting group.");
				}

				var owningGroup = _groupFactory.CreateOwningGroup(owned, include, exclude);
				baseGroupNode.AddGroupAfterThis(owningGroup);
				return RegisterAndSync(groupCode, owningGroup);
			}

			// Try to create group as a new base group
			if (baseGroup.ExtendsGroup(owned, include, exclude))
			{
				var owningGroup = _groupFactory.CreateOwningGroup(owned, include, exclude);
				baseGroup.AddGroupBeforeThis(owningGroup);
				foreach (var set in owned)
				{
					_ownedBase[set] = owningGroup;
				}
				return RegisterAndSync(groupCode, owningGroup);
			}

			throw new Exception("Conflicting group.");
		}

		public Group Get<TInclude, TExclude, TOwn>()
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
			where TOwn : IOwnSelector, new()
		{
			var group = _groupGenericLookup.Find<Tuple<TInclude, TExclude, TOwn>>();

			if (group != null)
			{
				group.EnsureSynced();
				return group;
			}

			var include = new TInclude().Select(_setRegistry);
			var exclude = new TExclude().Select(_setRegistry);
			var owned = new TOwn().Select(_setRegistry);

			group = Get(include, exclude, owned);

			_groupGenericLookup.Assign<Tuple<TInclude, TExclude, TOwn>>(group);

			return group;
		}

		private Group RegisterAndSync(int groupCode, Group group)
		{
			_groupCodeLookup.Add(groupCode, group);
			_allGroups.Add(group);
			group.EnsureSynced();
			return group;
		}

		private readonly HashSet<int> _duplicatesBuffer = new HashSet<int>();

		private bool ContainsDuplicates(SparseSet[] sets)
		{
			_duplicatesBuffer.Clear();
			foreach(var set in sets) 
			{
				if (!_duplicatesBuffer.Add(_setRegistry.IndexOf(set)))
				{
					return true;
				}
			}

			return false;
		}
	}
}
