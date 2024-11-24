namespace Massive
{
	public interface ICopyable<T> where T : ICopyable<T>
	{
		void CopyTo(ref T other);

		private static void ReflectionSupportForAOT()
		{
			_ = new MassiveCopyingDataSet<T>();
		}
	}
}
