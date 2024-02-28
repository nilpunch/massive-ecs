﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	public class Registry : IRegistry
	{
		private readonly Dictionary<Type, ISet> _pools;
		private readonly ISetFactory _setFactory;

		protected List<ISet> AllSets { get; }
		public ISet Entities { get; }

		public Registry(ISetFactory setFactory)
		{
			_setFactory = setFactory;
			_pools = new Dictionary<Type, ISet>();

			Entities = setFactory.CreateSet();
			AllSets = new List<ISet>() { Entities };
		}

		public Registry(int dataCapacity = Constants.DataCapacity)
			: this(new NormalSetFactory(dataCapacity))
		{
		}

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

			return ref GetOrCreateComponentPool<T>().Get(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has<T>(int entityId) where T : unmanaged
		{
			if (_pools.TryGetValue(typeof(T), out var component))
			{
				return component.IsAlive(entityId);
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add<T>(int entityId, T data = default) where T : unmanaged
		{
			if (ComponentMeta<T>.HasAnyFields)
			{
				GetOrCreateComponentPool<T>().Ensure(entityId, data);
			}
			else
			{
				GetOrCreateTagPool<T>().Ensure(entityId);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove<T>(int entityId) where T : unmanaged
		{
			if (_pools.TryGetValue(typeof(T), out var anySet))
			{
				anySet.Delete(entityId);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IDataSet<T> Component<T>() where T : unmanaged
		{
			if (!ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception($"Type has no fields! Use {nameof(Tag)} instead.");
			}

			return GetOrCreateComponentPool<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet Tag<T>() where T : unmanaged
		{
			if (ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception($"Type has fields! Use {nameof(Component)} instead.");
			}

			return GetOrCreateTagPool<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet AnySet<T>() where T : unmanaged
		{
			if (ComponentMeta<T>.HasAnyFields)
			{
				return GetOrCreateComponentPool<T>();
			}
			else
			{
				return GetOrCreateTagPool<T>();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private IDataSet<T> GetOrCreateComponentPool<T>() where T : unmanaged
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
		private ISet GetOrCreateTagPool<T>() where T : unmanaged
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