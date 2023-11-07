using System.Text;
using UnityEngine;

namespace Massive.Samples.Shooter
{
    public class PrefabFactory<TPrefab, TState> : IEntityFactory<TState> where TPrefab : Object, IEntity<TState> where TState : struct
    {
        private readonly Transform _parent;
        private readonly TPrefab _prefab;
        private readonly string _name;
        private readonly StringBuilder _nameBuilder;

        private int _objectIndex = 0;

        public PrefabFactory(TPrefab prefab, Transform parent, string name)
        {
            _prefab = prefab;
            _name = name;
            _parent = parent;
            _nameBuilder = new StringBuilder();
        }

        public PrefabFactory(TPrefab prefab, Transform parent) : this(prefab, parent, prefab.name)
        {
        }

        public PrefabFactory(TPrefab prefab) : this(prefab, null, prefab.name)
        {
        }

        public IEntity<TState> Create()
        {
            TPrefab instance = _parent ? Object.Instantiate(_prefab, _parent) : Object.Instantiate(_prefab);
            instance.name = _nameBuilder.Append(_name).Append(' ').Append(_objectIndex).ToString();
            _nameBuilder.Clear();
            _objectIndex++;
            return instance;
        }
    }
}
