using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="BitSet"/>.
	/// Resets data to default value for added elements.
	/// Used for unmanaged components.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class UnmanagedDataSet<T> : DataSet<T>
	{
		public T DefaultValue { get; }

		public UnmanagedDataSet(T defaultValue = default)
		{
			DefaultValue = defaultValue;
		}

		/// <summary>
		/// Resets data at the specified index.
		/// </summary>
		protected override void PrepareData(int id)
		{
			PagedData[id >> Constants.PageSizePower][id & Constants.PageSizeMinusOne] = DefaultValue;
		}
	}
}
