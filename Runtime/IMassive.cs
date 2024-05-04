namespace Massive
{
	public interface IMassive
	{
		/// <summary>
		/// Can be negative when there are absolutely no saved frames to restore data.
		/// </summary>
		int CanRollbackFrames { get; }

		/// <summary>
		/// Saves current frame data.
		/// </summary>
		void SaveFrame();

		/// <summary>
		/// Restores data from some frames ago.<br/>
		/// </summary>
		/// <param name="frames">Must be non-negative and not exceed <see cref="IMassive.CanRollbackFrames"/>.</param>
		void Rollback(int frames);
	}
}
