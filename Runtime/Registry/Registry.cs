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
		public Identifiers Entities { get; }

		public Registry(int dataCapacity = Constants.DataCapacity)
			: this(new NormalSetFactory(dataCapacity))
		{
		}

		protected Registry(ISetFactory setFactory)
		{
			SetFactory = setFactory;
			SetsLookup = new Dictionary<Type, ISet>();
			AllSets = new List<ISet>();

			Entities = setFactory.CreateIdentifiers();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Create()
		{
			return Entities.Create();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Destroy(int entityId)
		{
			Entities.Delete(entityId);

			foreach (var set in AllSets)
			{
				set.Delete(entityId);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add<T>(int entityId, T data = default) where T : struct
		{
			if (ComponentMeta<T>.HasAnyFields)
			{
				GetOrCreateComponents<T>().Ensure(entityId, data);
			}
			else
			{
				GetOrCreateTags<T>().Ensure(entityId);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove<T>(int entityId) where T : struct
		{
			if (SetsLookup.TryGetValue(typeof(T), out var anySet))
			{
				anySet.Delete(entityId);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has<T>(int entityId) where T : struct
		{
			if (SetsLookup.TryGetValue(typeof(T), out var component))
			{
				return component.IsAlive(entityId);
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get<T>(int entityId) where T : struct
		{
			if (!ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception("Type has no fields!");
			}

			return ref GetOrCreateComponents<T>().Get(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IDataSet<T> Components<T>() where T : struct
		{
			if (!ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception($"Type has no fields! Use {nameof(Tags)}<T>() instead.");
			}

			return GetOrCreateComponents<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet Tags<T>() where T : struct
		{
			if (ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception($"Type has fields! Use {nameof(Components)}<T>() instead.");
			}

			return GetOrCreateTags<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet AnySet<T>() where T : struct
		{
			if (ComponentMeta<T>.HasAnyFields)
			{
				return GetOrCreateComponents<T>();
			}
			else
			{
				return GetOrCreateTags<T>();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private IDataSet<T> GetOrCreateComponents<T>() where T : struct
		{
			var type = typeof(T);

			if (!SetsLookup.TryGetValue(type, out var components))
			{
				components = SetFactory.CreateDataSet<T>();
				SetsLookup.Add(type, components);
				AllSets.Add(components);
			}

			return (IDataSet<T>)components;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ISet GetOrCreateTags<T>() where T : struct
		{
			var type = typeof(T);

			if (!SetsLookup.TryGetValue(type, out var tags))
			{
				tags = SetFactory.CreateSet();
				SetsLookup.Add(type, tags);
				AllSets.Add(tags);
			}

			return tags;
		}
	}
}