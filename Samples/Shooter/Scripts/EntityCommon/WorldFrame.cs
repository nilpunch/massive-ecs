namespace Massive.Samples.Shooter
{
	public readonly ref struct WorldFrame
	{
		public readonly Frame<CharacterState> Characters;
		public readonly Frame<BulletState> Bullets;
		public readonly int CurrentFrame;
		public readonly int FramesPerSecond;

		public readonly float DeltaTime;

		public WorldFrame(Frame<CharacterState> characters, Frame<BulletState> bullets, int currentFrame, int framesPerSecond = 60)
		{
			Characters = characters;
			Bullets = bullets;
			CurrentFrame = currentFrame;
			FramesPerSecond = framesPerSecond;
			DeltaTime = 1f / FramesPerSecond;
		}
	}
}