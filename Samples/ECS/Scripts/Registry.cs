using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			foreach (var massive in _allSets)
			{
				massive.SaveFrame();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CreateEntity()
		{
			return _entities.Create().Id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CreateEntity<T>(T data) where T : struct
		{
			int id = _entities.Create().Id;
			Set(id, data);
			return id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DeleteEntity(int entity)
		{
			foreach (var massive in _allSets)
			{
				massive.Delete(entity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get<T>(int entity) where T : struct
		{
			if (!ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception("Type has no fields!");
			}

			return ref GetOrCreateComponents<T>().Get(entity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has<T>(int entity) where T : struct
		{
			if (_components.TryGetValue(ComponentMeta<T>.Id, out var componentMassive))
			{
				return componentMassive.IsAlive(entity);
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove<T>(int entity) where T : struct
		{
			GetOrCreateComponents<T>().Delete(entity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IDataSet<T> Components<T>() where T : struct
		{
			if (!ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception("Type has no fields! Use GetTags<T>() instead.");
			}

			return GetOrCreateComponents<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet Tags<T>() where T : struct
		{
			if (!ComponentMeta<T>.HasAnyFields)
			{
				throw new Exception("Type has fields! Use GetComponents<T>() instead.");
			}

			return GetOrCreateTags<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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