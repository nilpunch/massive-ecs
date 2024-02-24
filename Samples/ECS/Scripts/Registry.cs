using System;
using System.Collections.Generic;

namespace Massive.Samples.ECS
{
	public class Registry : IMassive
	{
		private readonly int _framesCapacity;
		private readonly int _entitiesCapacity;

		private readonly MassiveSparseSet _entities;
		private readonly Dictionary<int, IMassiveSet> _components;

		private readonly List<IMassiveSet> _allSets;

		public Registry(int framesCapacity = 121, int entitiesCapacity = 1000)
		{
			_framesCapacity = framesCapacity;
			_entitiesCapacity = entitiesCapacity;

			_entities = new MassiveSparseSet(framesCapacity, entitiesCapacity);
			_components = new Dictionary<int, IMassiveSet>();
			_allSets = new List<IMassiveSet> { _entities };
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
				massive.Delete(entity);
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

		public IDataSet<T> GetComponents<T>() where T : struct
		{
			if (!ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception("Type has no fields! Use GetTags<T>() instead.");
			}

			return GetOrCreateComponents<T>();
		}

		public ISet GetTags<T>() where T : struct
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

		private IDataSet<T> GetOrCreateComponents<T>() where T : struct
		{
			int id = ComponentMeta<T>.Id;

			if (!_components.TryGetValue(id, out var components))
			{
				components = new MassiveDataSet<T>(_framesCapacity, _entitiesCapacity);
				_components.Add(id, components);
				_allSets.Add(components);
			}

			return (IDataSet<T>)components;
		}

		private ISet GetOrCreateTags<T>() where T : struct
		{
			int id = ComponentMeta<T>.Id;

			if (!_components.TryGetValue(id, out var tags))
			{
				tags = new MassiveSparseSet(_framesCapacity, _entitiesCapacity);
				_components.Add(id, tags);
				_allSets.Add(tags);
			}

			return tags;
		}
	}
}