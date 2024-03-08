namespace Massive
{
	public interface IMassive
	{
		/// <summary>
		/// Can be negative, when there absolutely no saved frames to restore information.
		/// </summary>
		int CanRollbackFrames { get; }

		void SaveFrame();

		void Rollback(int frames);
	}
}