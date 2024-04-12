using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class Registry : IRegistry
	{
		public IGroupsController Groups { get; }
		private ISetFactory SetFactory { get; }
		private Dictionary<Type, ISet> SetsLookup { get; }
		protected List<ISet> AllSets { get; }
		public Entities Entities { get; }

		public event Action<ISet> SetCreated;

		public Registry(int dataCapacity = Constants.DataCapacity, bool storeTagsAsComponents = false)
			: this(new GroupsController(dataCapacity), new NormalSetFactory(dataCapacity, storeTagsAsComponents))
		{
		}

		protected Registry(IGroupsController groups, ISetFactory setFactory)
		{
			Groups = groups;
			SetFactory = setFactory;
			SetsLookup = new Dictionary<Type, ISet>();
			AllSets = new List<ISet>();

			Entities = setFactory.CreateIdentifiers();
			Entities.BeforeDeleted += OnBeforeDeleted;
		}

		private void OnBeforeDeleted(int id)
		{
			if (!Entities.IsAlive(id))
			{
				return;
			}

			for (var i = 0; i < AllSets.Count; i++)
			{
				AllSets[i].Remove(id);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IDataSet<T> Components<T>() where T : struct
		{
			if (Any<T>() is not IDataSet<T> dataSet)
			{
				throw new Exception($"Type has no associated data! Maybe use {nameof(Any)}<T>() instead.");
			}

			return dataSet;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet Any<T>() where T : struct
		{
			var type = typeof(T);

			if (!SetsLookup.TryGetValue(type, out var set))
			{
				set = SetFactory.CreateAppropriateSet<T>();
				SetsLookup.Add(type, set);
				AllSets.Add(set);
				SetCreated?.Invoke(set);
			}

			return set;
		}
	}
}