using System.Text;
using UnityEngine;

namespace MassiveData.Samples.Shooter
{
	public class EntityFactory<TState> where TState : struct
	{
		private readonly Transform _parent;
		private readonly EntityRoot<TState> _prefab;
		private readonly string _name;
		private readonly StringBuilder _nameBuilder;

		private int _objectIndex = 0;

		public EntityFactory(EntityRoot<TState> prefab, Transform parent, string name)
		{
			_prefab = prefab;
			_name = name;
			_parent = parent;
			_nameBuilder = new StringBuilder();
		}

		public EntityFactory(EntityRoot<TState> prefab, Transform parent) : this(prefab, parent, prefab.name)
		{
		}

		public EntityFactory(EntityRoot<TState> prefab) : this(prefab, null, prefab.name)
		{
		}

		public EntityRoot<TState> Create()
		{
			EntityRoot<TState> instance = _parent ? Object.Instantiate(_prefab, _parent) : Object.Instantiate(_prefab);
			instance.name = _nameBuilder.Append(_name).Append(' ').Append(_objectIndex).ToString();
			_nameBuilder.Clear();
			_objectIndex++;
			return instance;
		}
	}
}