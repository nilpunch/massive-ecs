using System;
using System.Collections.Generic;

namespace Massive.Samples.ECS
{
	public class MassiveWorld : IMassive
	{
		private readonly int _framesCapacity;
		private readonly int _entitiesCapacity;

		private readonly ComponentSparseSet _entities;

		private readonly Dictionary<int, IComponentMassive> _componentsLookup;
		private readonly List<IComponentMassive> _components;

		public int CanRollbackFrames => _entities.CanRollbackFrames;

		public MassiveWorld(int framesCapacity = 121, int entitiesCapacity = 1000)
		{
			_framesCapacity = framesCapacity;
			_entitiesCapacity = entitiesCapacity;
			_entities = new ComponentSparseSet(framesCapacity, entitiesCapacity);
			_componentsLookup = new Dictionary<int, IComponentMassive>();
			_components = new List<IComponentMassive> { _entities };
		}

		public int CreateEntity()
		{
			return _entities.Create().Id;
		}

		public void DeleteEntity(int entity)
		{
			foreach (var massive in _components)
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
			if (_componentsLookup.TryGetValue(ComponentMeta<T>.Id, out var componentMassive))
			{
				return componentMassive.IsAlive(entity);
			}
			
			return false;
		}

		public ComponentDataSet<T> GetComponents<T>() where T : struct
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

		public void SaveFrame()
		{
			foreach (var massive in _components)
			{
				massive.SaveFrame();
			}
		}

		public void Rollback(int frames)
		{
			foreach (var massive in _components)
			{
				if (massive.CanRollbackFrames >= 0)
				{
					massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
				}
			}
		}

		private ComponentDataSet<T> GetOrCreateComponents<T>() where T : struct
		{
			int id = ComponentMeta<T>.Id;

			if (!_componentsLookup.TryGetValue(id, out var components))
			{
				components = new ComponentDataSet<T>(_framesCapacity, _entitiesCapacity);
				_componentsLookup.Add(id, components);
				_components.Add(components);
			}

			return (ComponentDataSet<T>)components;
		}

		private ComponentSparseSet GetOrCreateTags<T>() where T : struct
		{
			int id = ComponentMeta<T>.Id;

			if (!_componentsLookup.TryGetValue(id, out var tags))
			{
				tags = new ComponentSparseSet(_framesCapacity, _entitiesCapacity);
				_componentsLookup.Add(id, tags);
				_components.Add(tags);
			}

			return (ComponentSparseSet)tags;
		}
	}
}