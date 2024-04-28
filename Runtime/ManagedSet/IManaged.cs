namespace Massive
{
	public interface IManaged<T> where T : IManaged<T>
	{
		void CopyTo(ref T other);

#if UNITY_2020_3_OR_NEWER
		[UnityEngine.Scripting.Preserve]
#endif
		private static void GenericsCompilerHint()
		{
			// ReSharper disable ObjectCreationAsStatement
			new MassiveManagedDataSet<T>();
		}
	}
}
