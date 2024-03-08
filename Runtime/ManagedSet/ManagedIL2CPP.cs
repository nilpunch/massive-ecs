namespace Massive
{
	public partial interface IManaged<T> where T : struct, IManaged<T>
	{
#if UNITY_2020_3_OR_NEWER
		[UnityEngine.Scripting.Preserve]
#endif
		static void GenericsCompilerHint()
		{
			new ManagedDataSet<T>();
			new MassiveManagedDataSet<T>();
		}
	}
}