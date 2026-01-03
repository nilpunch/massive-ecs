namespace Massive
{
	/// <summary>
	/// Defines a contract for objects that support saving and restoring snapshots of state.<br/>
	/// The term "frames" refers to snapshots of states.
	/// </summary>
	public interface IMassive
	{
		/// <summary>
		/// The number of frames that can be rolled back.<br/>
		/// Can be negative if there are no saved frames to restore data.
		/// </summary>
		int CanRollbackFrames { get; }

		/// <summary>
		/// Saves the current frame data.
		/// </summary>
		void SaveFrame();

		/// <summary>
		/// Restores data from the specified number of frames ago.<br/>
		/// A value of 0 restores data to the state of the last <see cref="IMassive.SaveFrame"/> call.
		/// </summary>
		/// <param name="frames">
		/// The number of frames to roll back. Must be non-negative and not exceed <see cref="IMassive.CanRollbackFrames"/>.
		/// </param>
		void Rollback(int frames);
	}
}
