using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	public class Registry : IRegistry
	{
		private readonly ISetFactory _setFactory;
		private readonly Dictionary<Type, ISet> _setsLookup;

		protected List<ISet> AllSets { get; }
		public Identifiers Entities { get; }

		public Registry(int dataCapacity = Constants.DataCapacity)
			: this(new NormalSetFactory(dataCapacity))
		{
		}

		protected Registry(ISetFactory setFactory)
		{
			_setFactory = setFactory;
			_setsLookup = new Dictionary<Type, ISet>();
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
		public void Add<T>(int entityId, T data = default) where T : unmanaged
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
		public void Remove<T>(int entityId) where T : unmanaged
		{
			if (_setsLookup.TryGetValue(typeof(T), out var anySet))
			{
				anySet.Delete(entityId);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has<T>(int entityId) where T : unmanaged
		{
			if (_setsLookup.TryGetValue(typeof(T), out var component))
			{
				return component.IsAlive(entityId);
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get<T>(int entityId) where T : unmanaged
		{
			if (!ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception("Type has no fields!");
			}

			return ref GetOrCreateComponents<T>().Get(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DataSet<T> Components<T>() where T : unmanaged
		{
			if (!ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception($"Type has no fields! Use {nameof(Tags)}<T>() instead.");
			}

			return GetOrCreateComponents<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet Tags<T>() where T : unmanaged
		{
			if (ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception($"Type has fields! Use {nameof(Components)}<T>() instead.");
			}

			return GetOrCreateTags<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet AnySet<T>() where T : unmanaged
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
		private DataSet<T> GetOrCreateComponents<T>() where T : unmanaged
		{
			var type = typeof(T);

			if (!_setsLookup.TryGetValue(type, out var components))
			{
				components = _setFactory.CreateDataSet<T>();
				_setsLookup.Add(type, components);
				AllSets.Add(components);
			}

			return (DataSet<T>)components;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private SparseSet GetOrCreateTags<T>() where T : unmanaged
		{
			var type = typeof(T);

			if (!_setsLookup.TryGetValue(type, out var tags))
			{
				tags = _setFactory.CreateSet();
				_setsLookup.Add(type, tags);
				AllSets.Add(tags);
			}

			return (SparseSet)tags;
		}
	}
}