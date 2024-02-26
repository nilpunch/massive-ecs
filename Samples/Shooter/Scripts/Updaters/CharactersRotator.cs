using Massive.ECS;
using UnityEngine;

namespace Massive.Samples.Shooter
{
	public class CharactersRotator : WorldUpdater
	{
		[SerializeField] private float _rotation = 400f;

		private View<CharacterState> _characters;

		public override void Init(Registry registry)
		{
			_characters = registry.View<CharacterState>();
		}

		public override void UpdateWorld(float deltaTime)
		{
			_characters.ForEach((ref CharacterState characterState) =>
			{
				characterState.Transform.Rotation *= Quaternion.AngleAxis(_rotation * deltaTime, Vector3.forward);
			});
		}
	}
}