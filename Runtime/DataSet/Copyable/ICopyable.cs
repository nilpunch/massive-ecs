namespace Massive
{
	public interface ICopyable<T> where T : ICopyable<T>
	{
		void CopyTo(ref T other);

		[UnityEngine.Scripting.Preserve]
		public static SetAndCloner CreateDataSetAndCloner(object defaultValue)
		{
			var set = new CopyingDataSet<T>((T)defaultValue);
			return new SetAndCloner(set, new CopyingDataSetCloner<T>(set));
		}
	}
}
