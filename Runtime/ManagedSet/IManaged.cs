namespace Massive
{
	public interface IManaged<T> where T : struct, IManaged<T>
	{
		void CopyTo(ref T destination);

#if UNITY_2020_3_OR_NEWER
		[UnityEngine.Scripting.Preserve]
#endif
		private static void GenericsCompilerHint()
		{
			// ReSharper disable ObjectCreationAsStatement
			new ManagedDataSet<T>();
			new MassiveManagedDataSet<T>();
		}
	}
}