using UnityEngine;

namespace Massive.Samples.Shooter
{
	public class CharactersRotator : WorldUpdater
	{
		[SerializeField] private float _rotation = 400f;

		private IRegistry _registry;
		private IReadOnlyDataSet<CharacterState> _characters;

		public override void Init(IRegistry registry)
		{
			_registry = registry;
			_characters = registry.Components<CharacterState>();
		}

		public override void UpdateWorld(float deltaTime)
		{
			var data = _characters.AliveData;
			for (var i = 0; i < data.Length; i++)
			{
				ref var characterState = ref data[i];
				characterState.Transform.Rotation *= Quaternion.AngleAxis(_rotation * deltaTime, Vector3.forward);
			}
		}
	}
}