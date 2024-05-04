namespace Massive
{
	public interface IManaged<T> where T : IManaged<T>
	{
		void CopyTo(ref T other);

		// This is a hint for IL2CPP
		private static void ReflectionSupportForAOT()
		{
			_ = new MassiveManagedDataSet<T>();
		}
	}
}
