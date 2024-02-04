namespace Massive.Samples.Shooter
{
	public readonly ref struct WorldFrame
	{
		public readonly MassiveData<CharacterState> Characters;
		public readonly MassiveData<BulletState> Bullets;
		public readonly int CurrentFrame;
		public readonly int FramesPerSecond;

		public readonly float DeltaTime;

		public WorldFrame(MassiveData<CharacterState> characters, MassiveData<BulletState> bullets, int currentFrame, int framesPerSecond = 60)
		{
			Characters = characters;
			Bullets = bullets;
			CurrentFrame = currentFrame;
			FramesPerSecond = framesPerSecond;
			DeltaTime = 1f / FramesPerSecond;
		}
	}
}