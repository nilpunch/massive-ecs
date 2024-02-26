using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	public class Registry : IMassive
	{
		private readonly int _framesCapacity;
		private readonly int _entitiesCapacity;

		private readonly MassiveSparseSet _entities;
		private readonly Dictionary<Type, IMassiveSet> _pools;
		private readonly List<IMassiveSet> _massives;

		public Registry(int framesCapacity = 121, int entitiesCapacity = 1000)
		{
			_framesCapacity = framesCapacity;
			_entitiesCapacity = entitiesCapacity;

			_entities = new MassiveSparseSet(framesCapacity, entitiesCapacity);
			_pools = new Dictionary<Type, IMassiveSet>();
			_massives = new List<IMassiveSet> { _entities };

			// Save first empty frame to ensure we can rollback to it
			_entities.SaveFrame();
		}

		public ISet Entities => _entities;

		public int CanRollbackFrames => _entities.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			foreach (var massive in _massives)
			{
				massive.SaveFrame();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			foreach (var massive in _massives)
			{
				massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity CreateEntity<T>(T data) where T : unmanaged
		{
			int id = _entities.Create().Id;
			Add(id, data);
			return new Entity(this, id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity CreateEntity()
		{
			return new Entity(this, _entities.Create().Id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DestroyEntity(int entityId)
		{
			foreach (var massive in _massives)
			{
				massive.Delete(entityId);
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
				throw new Exception("Type has no fields! Use GetTags<T>() instead.");
			}

			return GetOrCreateComponents<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet Tag<T>() where T : unmanaged
		{
			if (ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception("Type has fields! Use GetComponents<T>() instead.");
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
				components = new MassiveDataSet<T>(_framesCapacity, _entitiesCapacity);

				// Save first empty frame to ensure we can rollback to it
				components.SaveFrame();

				_pools.Add(type, components);
				_massives.Add(components);
			}

			return (IDataSet<T>)components;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ISet GetOrCreateTags<T>() where T : unmanaged
		{
			var type = typeof(T);

			if (!_pools.TryGetValue(type, out var tags))
			{
				tags = new MassiveSparseSet(_framesCapacity, _entitiesCapacity);

				// Save first empty frame to ensure we can rollback to it
				tags.SaveFrame();

				_pools.Add(type, tags);
				_massives.Add(tags);
			}

			return tags;
		}
	}
}