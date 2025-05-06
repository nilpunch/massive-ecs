using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.SparseSet"/>.
	/// Resets data to default value for added elements.
	/// Used for unmanaged components.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class UnmanagedDataSet<T> : DataSet<T>
	{
		public T DefaultValue { get; }

		public UnmanagedDataSet(int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous, T defaultValue = default)
			: base(pageSize, packing)
		{
			DefaultValue = defaultValue;
		}

		/// <summary>
		/// Ensures data exists at the specified index, and resets it.
		/// </summary>
		protected override void EnsureAndPrepareDataAt(int index)
		{
			Data.EnsurePageAt(index);
			Data[index] = DefaultValue;
		}

		/// <summary>
		/// Resets data at the specified index.
		/// </summary>
		protected override void PrepareDataAt(int index)
		{
			Data[index] = DefaultValue;
		}
	}
}
