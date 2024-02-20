using System;
using System.Collections.Generic;
using MassiveData;

namespace Massive.Samples.ECS
{
	public class MassiveWorld : IMassive
	{
		private readonly int _framesCapacity;
		private readonly int _entitiesCapacity;

		private readonly ExtendedMassiveSparseSet _entities;

		private readonly Dictionary<int, IExtendedMassive> _massivesLookup;
		private readonly List<IExtendedMassive> _massives;

		public int CanRollbackFrames => _entities.CanRollbackFrames;

		public MassiveWorld(int framesCapacity = 121, int entitiesCapacity = 1000)
		{
			_framesCapacity = framesCapacity;
			_entitiesCapacity = entitiesCapacity;
			_entities = new ExtendedMassiveSparseSet(framesCapacity, entitiesCapacity);
			_massivesLookup = new Dictionary<int, IExtendedMassive>();
			_massives = new List<IExtendedMassive> { _entities };
		}

		public int CreateEntity()
		{
			return _entities.Create().Id;
		}

		public void DeleteEntity(int entity)
		{
			foreach (var massive in _massives)
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

		public Massive<T> GetComponents<T>() where T : struct
		{
			if (!ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception("Type has no fields!");
			}

			return GetOrCreateComponents<T>();
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
			if (ComponentMeta<T>.HasAnyFields)
			{
				var components = GetOrCreateComponents<T>();
				components.Delete(entity);
			}
			else
			{
				var tags = GetOrCreateTags<T>();
				tags.Delete(entity);
			}
		}

		public void SaveFrame()
		{
			foreach (var massive in _massives)
			{
				massive.SaveFrame();
			}
		}

		public void Rollback(int frames)
		{
			foreach (var massive in _massives)
			{
				if (massive.CanRollbackFrames >= 0)
				{
					massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
				}
			}
		}

		private ExtendedMassive<T> GetOrCreateComponents<T>() where T : struct
		{
			int id = ComponentMeta<T>.Id;

			if (!_massivesLookup.TryGetValue(id, out var components))
			{
				components = new ExtendedMassive<T>(_framesCapacity, _entitiesCapacity);
				_massivesLookup.Add(id, components);
				_massives.Add(components);
			}

			return (ExtendedMassive<T>)components;
		}

		private ExtendedMassiveSparseSet GetOrCreateTags<T>() where T : struct
		{
			int id = ComponentMeta<T>.Id;

			if (!_massivesLookup.TryGetValue(id, out var tags))
			{
				tags = new ExtendedMassiveSparseSet(_framesCapacity, _entitiesCapacity);
				_massivesLookup.Add(id, tags);
				_massives.Add(tags);
			}

			return (ExtendedMassiveSparseSet)tags;
		}
	}
}