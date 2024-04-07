using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class Registry : IRegistry
	{
		public IGroupsController Groups { get; }
		public ISetFactory SetFactory { get; }
		public Dictionary<Type, ISet> SetsLookup { get; }
		public List<ISet> AllSets { get; }
		public Identifiers Entities { get; }

		public Registry(int dataCapacity = Constants.DataCapacity, bool storeTagsAsComponents = false)
			: this(new GroupsController(dataCapacity), new NormalSetFactory(dataCapacity, storeTagsAsComponents)) { }

		protected Registry(IGroupsController groups, ISetFactory setFactory)
		{
			Groups = groups;
			SetFactory = setFactory;
			SetsLookup = new Dictionary<Type, ISet>();
			AllSets = new List<ISet>();
			Entities = setFactory.CreateIdentifiers();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Identifier Create()
		{
			return Entities.Create();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Destroy(Identifier identifier)
		{
			if (!Entities.IsAlive(identifier))
			{
				return;
			}

			Destroy(identifier.Id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Destroy(int entityId)
		{
			Entities.Delete(entityId);

			for (var i = 0; i < AllSets.Count; i++)
			{
				AllSets[i].Remove(entityId);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(Identifier identifier)
		{
			return Entities.IsAlive(identifier);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int entityId)
		{
			return Entities.IsAlive(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add<T>(Identifier identifier, T data = default) where T : struct
		{
			if (!Entities.IsAlive(identifier))
			{
				return;
			}

			Add(identifier.Id, data);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add<T>(int entityId, T data = default) where T : struct
		{
			var set = GetOrCreateSet<T>();
			if (set is IDataSet<T> dataSet)
			{
				dataSet.Ensure(entityId, data);
			}
			else
			{
				set.Ensure(entityId);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove<T>(Identifier identifier) where T : struct
		{
			if (!Entities.IsAlive(identifier))
			{
				return;
			}

			Remove<T>(identifier.Id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove<T>(int entityId) where T : struct
		{
			if (SetsLookup.TryGetValue(typeof(T), out var set))
			{
				set.Remove(entityId);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has<T>(Identifier identifier) where T : struct
		{
			if (!Entities.IsAlive(identifier))
			{
				return false;
			}

			return Has<T>(identifier.Id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has<T>(int entityId) where T : struct
		{
			if (SetsLookup.TryGetValue(typeof(T), out var set))
			{
				return set.IsAlive(entityId);
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get<T>(Identifier identifier) where T : struct
		{
			if (!Entities.IsAlive(identifier))
			{
				throw new Exception("Entity is not alive!");
			}

			return ref Get<T>(identifier.Id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get<T>(int entityId) where T : struct
		{
			if (GetOrCreateSet<T>() is not IDataSet<T> dataSet)
			{
				throw new Exception("Type has no associated data!");
			}

			return ref dataSet.Get(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IDataSet<T> Components<T>() where T : struct
		{
			if (GetOrCreateSet<T>() is not IDataSet<T> dataSet)
			{
				throw new Exception($"Type has no associated data! Use {nameof(Any)}<T>() instead.");
			}

			return dataSet;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet Any<T>() where T : struct
		{
			return GetOrCreateSet<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ISet GetOrCreateSet<T>() where T : struct
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