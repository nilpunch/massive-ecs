using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="Massive.UnmanagedDataSet{T}"/>.
	/// Resets data to default value for added elements.
	/// Used for unmanaged components.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class MassiveUnmanagedDataSet<T> : MassiveDataSet<T>
	{
		public T DefaultValue { get; }

		public MassiveUnmanagedDataSet(int framesCapacity = Constants.DefaultFramesCapacity, int pageSize = Constants.DefaultPageSize,
			Packing packing = Packing.Continuous, T defaultValue = default)
			: base(framesCapacity, pageSize, packing)
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
