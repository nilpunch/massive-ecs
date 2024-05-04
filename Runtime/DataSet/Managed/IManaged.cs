namespace Massive
{
	public interface IManaged<T> where T : IManaged<T>
	{
		void CopyTo(ref T other);

		/// <summary>
		/// This is a hint for IL2CPP to compile
		/// <see cref="MassiveManagedDataSet{T}"/> for each <see cref="IManaged{T}"/> implementation.
		/// This will allow to construct <see cref="MassiveManagedDataSet{T}"/> using reflection.
		/// </summary>
		private static void ReflectionSupportForAOT()
		{
			// ReSharper disable ObjectCreationAsStatement
			new MassiveManagedDataSet<T>();
		}
	}
}
