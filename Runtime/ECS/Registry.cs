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
		private readonly Dictionary<int, IMassiveSet> _pools;
		private readonly List<IMassiveSet> _massives;

		public Registry(int framesCapacity = 121, int entitiesCapacity = 1000)
		{
			_framesCapacity = framesCapacity;
			_entitiesCapacity = entitiesCapacity;

			_entities = new MassiveSparseSet(framesCapacity, entitiesCapacity);
			_pools = new Dictionary<int, IMassiveSet>();
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
		public int CreateEntity()
		{
			return _entities.Create().Id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CreateEntity<T>(T data) where T : unmanaged
		{
			int id = _entities.Create().Id;
			Add(id, data);
			return id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DeleteEntity(int entity)
		{
			foreach (var massive in _massives)
			{
				massive.Delete(entity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get<T>(int entity) where T : unmanaged
		{
			if (!ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception("Type has no fields!");
			}

			return ref GetOrCreateComponents<T>().Get(entity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has<T>(int entity) where T : unmanaged
		{
			if (_pools.TryGetValue(ComponentMeta<T>.Id, out var componentMassive))
			{
				return componentMassive.IsAlive(entity);
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add<T>(int entity, T data = default) where T : unmanaged
		{
			if (ComponentMeta<T>.HasAnyFields)
			{
				var components = GetOrCreateComponents<T>();
				components.Ensure(entity, data);
			}
			else
			{
				var tags = GetOrCreateTags<T>();
				tags.Ensure(entity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove<T>(int entity) where T : unmanaged
		{
			GetOrCreateComponents<T>().Delete(entity);
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
			int id = ComponentMeta<T>.Id;

			if (!_pools.TryGetValue(id, out var components))
			{
				components = new MassiveDataSet<T>(_framesCapacity, _entitiesCapacity);

				// Save first empty frame to ensure we can rollback to it
				components.SaveFrame();

				_pools.Add(id, components);
				_massives.Add(components);
			}

			return (IDataSet<T>)components;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ISet GetOrCreateTags<T>() where T : unmanaged
		{
			int id = ComponentMeta<T>.Id;

			if (!_pools.TryGetValue(id, out var tags))
			{
				tags = new MassiveSparseSet(_framesCapacity, _entitiesCapacity);

				// Save first empty frame to ensure we can rollback to it
				tags.SaveFrame();

				_pools.Add(id, tags);
				_massives.Add(tags);
			}

			return tags;
		}
	}
}