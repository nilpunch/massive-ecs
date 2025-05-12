namespace Massive
{
	public enum MemoryInit
	{
		/// <summary>
		/// Fills data with default value on allocation.
		/// </summary>
		Clear,

		/// <summary>
		/// Uninitialized data can improve performance, but results in the contents of the array elements being undefined.
		/// In performance sensitive code it can make sense to use <see cref="MemoryInit.Uninitialized"/>,
		/// if you are writing to the entire array right after creating it without reading any of the elements first.
		/// </summary>
		Uninitialized,
	}
}
