using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public struct TestState<T1, T2, T3>
	{
		public T1 Value1;
		public T2 Value2;
		public T3 Value3;

		public Quaternion Data1;
		public Quaternion Data2;
		public Quaternion Data3;
	}

	public struct TestState
	{
		public Vector3 Position;

		public Quaternion Data1;
		public Quaternion Data2;
		public Quaternion Data3;
		public Quaternion Data4;
		public Quaternion Data5;
		public Quaternion Data6;
	}

	public struct TestState2
	{
		public Vector3 Position;

		public Quaternion Data1;
		public Quaternion Data2;
		public Quaternion Data3;
		public Quaternion Data4;
		public Quaternion Data5;
		public Quaternion Data6;
	}
}