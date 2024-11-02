using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class GroupRegistry
	{
		private readonly GenericLookup<Group> _groupLookup = new GenericLookup<Group>();
		private readonly Dictionary<int, Group> _groupsByCode = new Dictionary<int, Group>();
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
			get => _groupLookup.All;
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

			if (_groupsByCode.TryGetValue(groupCode, out var group))
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
			var group = _groupLookup.Find<Tuple<TInclude, TExclude, TOwn>>();

			if (group != null)
			{
				group.EnsureSynced();
				return group;
			}

			var owned = new TOwn().Select(_setRegistry);
			var include = new TInclude().Select(_setRegistry);
			var exclude = new TExclude().Select(_setRegistry);

			int ownedCode = owned.GetUnorderedHashCode(_setRegistry);
			int includeCode = include.GetUnorderedHashCode(_setRegistry);
			int excludeCode = exclude.GetUnorderedHashCode(_setRegistry);
			int groupCode = MathHelpers.CombineHashes(MathHelpers.CombineHashes(ownedCode, includeCode), excludeCode);

			if (_groupsByCode.TryGetValue(groupCode, out group))
			{
				return RegisterAndSync<Tuple<TInclude, TExclude, TOwn>>(groupCode, group);
			}

			if (Array.Exists(owned, set => set.PackingMode == PackingMode.WithHoles))
			{
				throw new Exception($"Sets with direct storage are not supported for owning: <{typeof(TOwn).GetFullGenericName()}>.");
			}

			// If non-owning, then just create new one
			if (owned.Length == 0)
			{
				var entitiesIfNoIncludes = include.Length == 0 ? _entities : null;
				var nonOwningGroup = _groupFactory.CreateNonOwningGroup(include, exclude, entitiesIfNoIncludes);
				return RegisterAndSync<Tuple<TInclude, TExclude, TOwn>>(groupCode, nonOwningGroup);
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
				return RegisterAndSync<Tuple<TInclude, TExclude, TOwn>>(groupCode, owningGroup);
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
					throw new Exception($"Conflicting group: <{typeof(TOwn).GetFullGenericName()}, " +
					                    $"{typeof(TInclude).GetFullGenericName()}, {typeof(TExclude).GetFullGenericName()}>.");
				}

				var owningGroup = _groupFactory.CreateOwningGroup(owned, include, exclude);
				baseGroupNode.AddGroupAfterThis(owningGroup);
				return RegisterAndSync<Tuple<TInclude, TExclude, TOwn>>(groupCode, owningGroup);
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
				return RegisterAndSync<Tuple<TInclude, TExclude, TOwn>>(groupCode, owningGroup);
			}

			throw new Exception($"Conflicting group: <{typeof(TOwn).GetFullGenericName()}, " +
			                    $"{typeof(TInclude).GetFullGenericName()}, {typeof(TExclude).GetFullGenericName()}>.");
		}

		public Group GetReflection(Type includeSelector, Type excludeSelector, Type ownSelector)
		{
			var groupKey = typeof(Tuple<,,>).MakeGenericType(includeSelector, excludeSelector, typeof(None));

			var group = _groupLookup.Find(groupKey);

			if (group != null)
			{
				group.EnsureSynced();
				return group;
			}

			var include = ((IIncludeSelector)Activator.CreateInstance(includeSelector)).Select(_setRegistry);
			var exclude = ((IExcludeSelector)Activator.CreateInstance(excludeSelector)).Select(_setRegistry);
			var owned = ((IOwnSelector)Activator.CreateInstance(ownSelector)).Select(_setRegistry);

			int ownedCode = owned.GetUnorderedHashCode(_setRegistry);
			int includeCode = include.GetUnorderedHashCode(_setRegistry);
			int excludeCode = exclude.GetUnorderedHashCode(_setRegistry);
			int groupCode = MathHelpers.CombineHashes(MathHelpers.CombineHashes(ownedCode, includeCode), excludeCode);

			if (_groupsByCode.TryGetValue(groupCode, out group))
			{
				return RegisterAndSync(groupKey, groupCode, group);
			}

			if (Array.Exists(owned, set => set.PackingMode == PackingMode.WithHoles))
			{
				throw new Exception($"Sets with direct storage are not supported for owning: <{ownSelector.GetFullGenericName()}>.");
			}

			// If non-owning, then just create new one
			if (owned.Length == 0)
			{
				var nonOwningGroup = _groupFactory.CreateNonOwningGroup(include, exclude);
				return RegisterAndSync(groupKey, groupCode, nonOwningGroup);
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
				return RegisterAndSync(groupKey, groupCode, owningGroup);
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
					throw new Exception($"Conflicting group: <{ownSelector.GetFullGenericName()}, " +
					                    $"{includeSelector.GetFullGenericName()}, {excludeSelector.GetFullGenericName()}>.");
				}

				var owningGroup = _groupFactory.CreateOwningGroup(owned, include, exclude);
				baseGroupNode.AddGroupAfterThis(owningGroup);
				return RegisterAndSync(groupKey, groupCode, owningGroup);
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
				return RegisterAndSync(groupKey, groupCode, owningGroup);
			}

			throw new Exception($"Conflicting group: <{ownSelector.GetFullGenericName()}, " +
			                    $"{includeSelector.GetFullGenericName()}, {excludeSelector.GetFullGenericName()}>.");
		}

		public (Type IncludeSelector, Type ExcludeSelector, Type OwnSelector) GetSelectorsOfGroup(Group group)
		{
			var groupKey = _groupLookup.GetKey(group);
			var genericArguments = groupKey.GetGenericArguments();
			return (genericArguments[0], genericArguments[1], genericArguments[2]);
		}

		private Group RegisterAndSync(int groupCode, Group group)
		{
			_groupsByCode.Add(groupCode, group);
			group.EnsureSynced();
			return group;
		}

		private Group RegisterAndSync<TGroupKey>(int groupCode, Group group)
		{
			_groupsByCode.Add(groupCode, group);
			_groupLookup.Assign<TGroupKey>(group);
			group.EnsureSynced();
			return group;
		}

		private Group RegisterAndSync(Type groupKey, int groupCode, Group group)
		{
			_groupsByCode.Add(groupCode, group);
			_groupLookup.Assign(groupKey, group);
			group.EnsureSynced();
			return group;
		}

		private readonly HashSet<int> _duplicatesBuffer = new HashSet<int>();
		public bool ContainsDuplicates(SparseSet[] sets)
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
