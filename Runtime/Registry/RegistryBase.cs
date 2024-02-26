using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	public class RegistryBase<TSet> : IRegistry where TSet : ISet
	{
		protected TSet Entities { get; }
		protected List<TSet> AllSets { get; }

		private readonly Dictionary<Type, TSet> _pools;
		private readonly ISetFactory<TSet> _setFactory;

		public RegistryBase(ISetFactory<TSet> setFactory)
		{
			_setFactory = setFactory;
			_pools = new Dictionary<Type, TSet>();

			Entities = setFactory.CreateSet();
			AllSets = new List<TSet>() { Entities };
		}

		ISet IRegistry.Entities => Entities;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Create()
		{
			return Entities.Create().Id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Destroy(int entityId)
		{
			foreach (var set in AllSets)
			{
				set.Delete(entityId);
			}
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
		public bool Has<T>(int entityId) where T : unmanaged
		{
			if (_pools.TryGetValue(typeof(T), out var componentMassive))
			{
				return componentMassive.IsAlive(entityId);
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add<T>(int entityId, T data = default) where T : unmanaged
		{
			if (ComponentMeta<T>.HasAnyFields)
			{
				var components = GetOrCreateComponents<T>();
				components.Ensure(entityId, data);
			}
			else
			{
				var tags = GetOrCreateTags<T>();
				tags.Ensure(entityId);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove<T>(int entityId) where T : unmanaged
		{
			GetOrCreateComponents<T>().Delete(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IDataSet<T> Component<T>() where T : unmanaged
		{
			if (!ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception($"Type has no fields! Use {nameof(Tag)} instead.");
			}

			return GetOrCreateComponents<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet Tag<T>() where T : unmanaged
		{
			if (ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception($"Type has fields! Use {nameof(Component)} instead.");
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
		private IDataSet<T> GetOrCreateComponents<T>() where T : unmanaged
		{
			var type = typeof(T);

			if (!_pools.TryGetValue(type, out var components))
			{
				components = _setFactory.CreateDataSet<T>();
				_pools.Add(type, components);
				AllSets.Add(components);
			}

			return (IDataSet<T>)components;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ISet GetOrCreateTags<T>() where T : unmanaged
		{
			var type = typeof(T);

			if (!_pools.TryGetValue(type, out var tags))
			{
				tags = _setFactory.CreateSet();
				_pools.Add(type, tags);
				AllSets.Add(tags);
			}

			return tags;
		}
	}
}