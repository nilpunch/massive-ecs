using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class Registry : IRegistry
	{
		private ISetFactory SetFactory { get; }
		private Dictionary<Type, ISet> SetsLookup { get; }

		protected List<ISet> AllSets { get; }

		public IGroupsController Groups { get; }
		public IEntities Entities { get; }

		public Registry(int dataCapacity = Constants.DataCapacity, bool storeEmptyTypesAsDataSets = false)
			: this(new GroupsController(dataCapacity), new Entities(dataCapacity), new NormalSetFactory(dataCapacity, storeEmptyTypesAsDataSets))
		{
		}

		protected Registry(IGroupsController groups, IEntities entities, ISetFactory setFactory)
		{
			Groups = groups;
			Entities = entities;
			SetFactory = setFactory;
			SetsLookup = new Dictionary<Type, ISet>();
			AllSets = new List<ISet>();

			Entities.BeforeDestroyed += UnassignFromAllSets;
		}

		private void UnassignFromAllSets(int id)
		{
			for (var i = 0; i < AllSets.Count; i++)
			{
				AllSets[i].Unassign(id);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IDataSet<T> Components<T>()
		{
			if (Any<T>() is not IDataSet<T> dataSet)
			{
				throw new Exception($"Type has no associated data! Maybe use {nameof(Any)}<T>() instead.");
			}

			return dataSet;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet Any<T>()
		{
			var type = typeof(T);

			if (!SetsLookup.TryGetValue(type, out var set))
			{
				set = SetFactory.CreateAppropriateSet<T>();
				SetsLookup.Add(type, set);
				AllSets.Add(set);
			}

			return set;
		}
	}
}
