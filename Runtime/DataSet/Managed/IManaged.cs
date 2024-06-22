namespace Massive
{
	public interface IManaged<T> : ICopyable<T> where T : IManaged<T>
	{
		// This is a hint for IL2CPP
		private static void ReflectionSupportForAOT()
		{
			_ = new MassiveManagedDataSet<T>();
		}
	}
}
