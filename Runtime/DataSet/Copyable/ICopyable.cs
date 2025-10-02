namespace Massive
{
	public interface ICopyable<T> where T : ICopyable<T>
	{
		void CopyTo(ref T other);

		[UnityEngine.Scripting.Preserve]
		private static void ReflectionSupportForAOT()
		{
			_ = new CopyingDataSet<T>();
			_ = new CopyingDataSetCloner<T>(null);
		}
	}
}
