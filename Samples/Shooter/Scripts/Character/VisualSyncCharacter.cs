namespace MassiveData.Samples.Shooter
{
	public class VisualSyncCharacter : VisualSync<CharacterState>
	{
		protected override void TransformFromState(in CharacterState state, out EntityTransform transform)
		{
			transform = state.Transform;
		}
	}
}