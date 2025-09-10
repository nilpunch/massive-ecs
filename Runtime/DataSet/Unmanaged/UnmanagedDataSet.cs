using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="SparseSet"/>.
	/// Resets data to default value for added elements.
	/// Used for unmanaged components.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class UnmanagedDataSet<T> : DataSet<T>
	{
		public T DefaultValue { get; }

		public UnmanagedDataSet(int pageSize = Constants.DefaultPageSize, T defaultValue = default)
			: base(pageSize)
		{
			DefaultValue = defaultValue;
		}

		/// <summary>
		/// Resets data at the specified index.
		/// </summary>
		protected override void PrepareData(int blockIndex, int mod64)
		{
			var block = Blocks[blockIndex];
			PagedData[block.PageIndex][block.StartInPage + mod64] = DefaultValue;
		}
	}
}
