using System;
using System.Collections.Generic;

namespace Massive.Samples.ECS
{
	public class World : IMassive
	{
		private readonly int _framesCapacity;
		private readonly int _entitiesCapacity;

		private readonly EcsSparseSet _entities;
		private readonly Dictionary<int, IEcsSet> _components;

		private readonly List<IEcsSet> _allSets;

		public World(int framesCapacity = 121, int entitiesCapacity = 1000)
		{
			_framesCapacity = framesCapacity;
			_entitiesCapacity = entitiesCapacity;

			_entities = new EcsSparseSet(framesCapacity, entitiesCapacity);
			_components = new Dictionary<int, IEcsSet>();
			_allSets = new List<IEcsSet> { _entities };
		}

		public int CanRollbackFrames => _entities.CanRollbackFrames;

		public void SaveFrame()
		{
			foreach (var massive in _allSets)
			{
				massive.SaveFrame();
			}
		}

		public void Rollback(int frames)
		{
			foreach (var massive in _allSets)
			{
				if (massive.CanRollbackFrames >= 0)
				{
					massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
				}
			}
		}

		public int CreateEntity()
		{
			return _entities.Create().Id;
		}

		public void DeleteEntity(int entity)
		{
			foreach (var massive in _allSets)
			{
				massive.DeleteById(entity);
			}
		}

		public ref T Get<T>(int entity) where T : struct
		{
			if (!ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception("Type has no fields!");
			}

			return ref GetOrCreateComponents<T>().Get(entity);
		}

		public bool Has<T>(int entity) where T : struct
		{
			if (_components.TryGetValue(ComponentMeta<T>.Id, out var componentMassive))
			{
				return componentMassive.IsAlive(entity);
			}

			return false;
		}

		public EcsDataSet<T> GetComponents<T>() where T : struct
		{
			if (!ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception("Type has no fields! Use GetTags<T>() instead.");
			}

			return GetOrCreateComponents<T>();
		}

		public SparseSet GetTags<T>() where T : struct
		{
			if (!ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception("Type has fields! Use GetComponents<T>() instead.");
			}

			return GetOrCreateTags<T>();
		}

		public void Set<T>(int entity, T data = default) where T : struct
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

		public void Remove<T>(int entity) where T : struct
		{
			GetOrCreateComponents<T>().Delete(entity);
		}

		private EcsDataSet<T> GetOrCreateComponents<T>() where T : struct
		{
			int id = ComponentMeta<T>.Id;

			if (!_components.TryGetValue(id, out var components))
			{
				components = new EcsDataSet<T>(_framesCapacity, _entitiesCapacity);
				_components.Add(id, components);
				_allSets.Add(components);
			}

			return (EcsDataSet<T>)components;
		}

		private EcsSparseSet GetOrCreateTags<T>() where T : struct
		{
			int id = ComponentMeta<T>.Id;

			if (!_components.TryGetValue(id, out var tags))
			{
				tags = new EcsSparseSet(_framesCapacity, _entitiesCapacity);
				_components.Add(id, tags);
				_allSets.Add(tags);
			}

			return (EcsSparseSet)tags;
		}
	}
}