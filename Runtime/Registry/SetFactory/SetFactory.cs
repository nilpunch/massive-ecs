namespace Massive
{
	internal static class SetFactory<T>
	{
		internal static void CompilerHint()
		{
		}

#if UNITY_2020_3_OR_NEWER
		[UnityEngine.Scripting.Preserve]
#endif
		private static void VirtualGenericsCompilerHint()
		{
			// ReSharper disable ReturnValueOfPureMethodIsNotUsed
			new NormalSetFactory().CreateAppropriateSet<T>();
			new MassiveSetFactory().CreateAppropriateSet<T>();
		}
	}
}
