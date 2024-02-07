namespace MassiveData.Samples.Shooter
{
	public readonly ref struct WorldFrame
	{
		public readonly Massive<CharacterState> Characters;
		public readonly Massive<BulletState> Bullets;
		public readonly int CurrentFrame;
		public readonly int FramesPerSecond;

		public readonly float DeltaTime;

		public WorldFrame(Massive<CharacterState> characters, Massive<BulletState> bullets, int currentFrame, int framesPerSecond = 60)
		{
			Characters = characters;
			Bullets = bullets;
			CurrentFrame = currentFrame;
			FramesPerSecond = framesPerSecond;
			DeltaTime = 1f / FramesPerSecond;
		}
	}
}