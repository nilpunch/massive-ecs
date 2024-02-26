using Massive.ECS;
using UnityEngine;

namespace Massive.Samples.Shooter
{
	public class CharactersRotator : WorldUpdater
	{
		[SerializeField] private float _rotation = 400f;

		private View<CharacterState> _characters;

		public override void Init(MassiveRegistry registry)
		{
			_characters = registry.View<CharacterState>();
		}

		public override void UpdateWorld(float deltaTime)
		{
			foreach (var character in _characters)
			{
				ref CharacterState characterState = ref character.Get<CharacterState>();
				characterState.Transform.Rotation *= Quaternion.AngleAxis(_rotation * deltaTime, Vector3.forward);
			}
		}
	}
}