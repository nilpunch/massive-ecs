namespace Massive.Samples.Shooter
{
	public readonly ref struct World
	{
		public readonly Frame<CharacterState> Characters;
		public readonly Frame<BulletState> Bullets;
		public readonly int CurrentFrame;
		public readonly int FramesPerSecond;

		public readonly float DeltaTime;

		public World(Frame<CharacterState> characters, Frame<BulletState> bullets, int currentFrame, int framesPerSecond)
		{
			Characters = characters;
			Bullets = bullets;
			CurrentFrame = currentFrame;
			FramesPerSecond = framesPerSecond;
			DeltaTime = 1f / FramesPerSecond;
		}
	}
	
	public interface IWorldComponent<TState>
	{
		void UpdateState(World world, ref TState state);
	}
}