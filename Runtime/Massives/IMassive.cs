namespace Massive
{
	public interface IMassive
	{
		int CanRollbackFrames { get; }

		void SaveFrame();

		void Rollback(int frames);
	}
}