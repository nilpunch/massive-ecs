#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class GroupRegistry
	{
		private readonly GenericLookup<Group> _genericLookup = new GenericLookup<Group>();
		private readonly Dictionary<int, Group> _codeLookup = new Dictionary<int, Group>();
		private readonly FastList<Group> _allGroups = new FastList<Group>();

		private readonly Entities _entities;
		private readonly SetRegistry _setRegistry;
		private readonly IGroupFactory _groupFactory;

		public GroupRegistry(SetRegistry setRegistry, IGroupFactory groupFactory, Entities entities)
		{
			_setRegistry = setRegistry;
			_groupFactory = groupFactory;
			_entities = entities;
		}

		public FastList<Group> All
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _allGroups;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Group Get<TInclude, TExclude>()
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			var group = _genericLookup.Find<Tuple<TInclude, TExclude>>();

			if (group != null)
			{
				group.EnsureSynced();
				return group;
			}

			var included = new TInclude().Select(_setRegistry);
			var excluded = new TExclude().Select(_setRegistry);

			group = Get(included, excluded);

			_genericLookup.Assign<Tuple<TInclude, TExclude>>(group);

			return group;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Group Get(SparseSet[] included = null, SparseSet[] excluded = null)
		{
			included ??= Array.Empty<SparseSet>();
			excluded ??= Array.Empty<SparseSet>();

			Assert.NoConflictsInFilter(included, excluded);
			Assert.ContainsDuplicates(included, "Included contains duplicate sets!");
			Assert.ContainsDuplicates(excluded, "Excluded contains duplicate sets!");

			var includeCode = SetUtils.GetUnorderedHashCode(included, _setRegistry);
			var excludeCode = SetUtils.GetUnorderedHashCode(excluded, _setRegistry);
			var fullCode = MathUtils.CombineHashes(includeCode, excludeCode);

			if (_codeLookup.TryGetValue(fullCode, out var group))
			{
				group.EnsureSynced();
				return group;
			}

			var entitiesIfNoIncludes = included.Length == 0 ? _entities : null;
			group = _groupFactory.CreateGroup(included, excluded, entitiesIfNoIncludes);
			_codeLookup.Add(fullCode, group);
			_allGroups.Add(group);
			group.EnsureSynced();
			return group;
		}
	}
}
