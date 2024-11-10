using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class ReactiveRegistry
	{
		private readonly GenericLookup<ReactiveFilter> _genericLookup = new GenericLookup<ReactiveFilter>();
		private readonly Dictionary<int, ReactiveFilter> _codeLookup = new Dictionary<int, ReactiveFilter>();
		private readonly FastList<ReactiveFilter> _allReactiveFilters = new FastList<ReactiveFilter>();

		private readonly Entities _entities;
		private readonly SetRegistry _setRegistry;
		private readonly IReactiveFactory _reactiveFactory;

		public ReactiveRegistry(SetRegistry setRegistry, IReactiveFactory reactiveFactory, Entities entities)
		{
			_setRegistry = setRegistry;
			_reactiveFactory = reactiveFactory;
			_entities = entities;
		}

		public ReadOnlySpan<ReactiveFilter> All
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _allReactiveFilters.ReadOnlySpan;
		}

		public ReactiveFilter Get(SparseSet[] included = null, SparseSet[] excluded = null)
		{
			included ??= Array.Empty<SparseSet>();
			excluded ??= Array.Empty<SparseSet>();

			Filter.ThrowIfConflicting(included, excluded, "Conflicting include and exclude filter!");
			ThrowIfContainsDuplicates(included, "Included contains duplicate sets!");
			ThrowIfContainsDuplicates(excluded, "Excluded contains duplicate sets!");

			var includeCode = included.GetUnorderedHashCode(_setRegistry);
			var excludeCode = excluded.GetUnorderedHashCode(_setRegistry);
			var fullCode = MathUtils.CombineHashes(includeCode, excludeCode);

			if (_codeLookup.TryGetValue(fullCode, out var reactiveFilter))
			{
				reactiveFilter.EnsureSynced();
				return reactiveFilter;
			}

			var entitiesIfNoIncludes = included.Length == 0 ? _entities : null;
			reactiveFilter = _reactiveFactory.CreateReactiveFilter(included, excluded, entitiesIfNoIncludes);
			return RegisterAndSync(fullCode, reactiveFilter);
		}

		public ReactiveFilter Get<TInclude, TExclude>()
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

		private ReactiveFilter RegisterAndSync(int groupCode, ReactiveFilter group)
		{
			_codeLookup.Add(groupCode, group);
			_allReactiveFilters.Add(group);
			group.EnsureSynced();
			return group;
		}

		private readonly HashSet<int> _duplicatesBuffer = new HashSet<int>();

		private void ThrowIfContainsDuplicates(SparseSet[] sets, string errorMessage)
		{
			_duplicatesBuffer.Clear();
			foreach (var set in sets)
			{
				if (!_duplicatesBuffer.Add(_setRegistry.IndexOf(set)))
				{
					throw new Exception(errorMessage);
				}
			}
		}
	}
}
