namespace Massive
{
	public enum Packing : byte
	{
		/// <summary>
		/// When an element is removed, its position is filled with the last element in the packed array.
		/// </summary>
		Continuous,

		/// <summary>
		/// When an element is removed, its position is left as a hole in the packed array.
		/// Holes are filled automatically when new elements are added.
		/// </summary>
		WithHoles,

		/// <summary>
		/// When an element is removed, its position is left as a hole in the packed array.
		/// Holes persist until manually compacted.
		/// </summary>
		WithPersistentHoles,
	}
}
