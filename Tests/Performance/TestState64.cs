using UnityEngine;

namespace Massive.PerformanceTests
{
	/// <summary>
	/// Empty type.
	/// </summary>
	public struct TestTag<T1, T2, T3>
	{
	}

	/// <summary>
	/// Empty type.
	/// </summary>
	public struct TestTag
	{
	}

	/// <summary>
	/// 64 Byte structure.
	/// </summary>
	public struct TestState64<T1, T2, T3>
	{
		public Quaternion Bytes16_1;
		public Quaternion Bytes16_2;
		public Quaternion Bytes16_3;
		public Quaternion Bytes16_4;
	}

	/// <summary>
	/// 64 Byte structure.
	/// </summary>
	public struct TestState64
	{
		public Quaternion Bytes16_1;
		public Quaternion Bytes16_2;
		public Quaternion Bytes16_3;
		public Quaternion Bytes16_4;
	}

	/// <summary>
	/// 64 Byte structure.
	/// </summary>
	public struct TestState64Stable : IStable
	{
		public Quaternion Bytes16_1;
		public Quaternion Bytes16_2;
		public Quaternion Bytes16_3;
		public Quaternion Bytes16_4;
	}
	
	/// <summary>
	/// 64 Byte structure.
	/// </summary>
	public struct TestState64Stable_2 : IStable
	{
		public Quaternion Bytes16_1;
		public Quaternion Bytes16_2;
		public Quaternion Bytes16_3;
		public Quaternion Bytes16_4;
	}

	/// <summary>
	/// 64 Byte structure.
	/// </summary>
	public struct TestState64_2
	{
		public Quaternion Bytes16_1;
		public Quaternion Bytes16_2;
		public Quaternion Bytes16_3;
		public Quaternion Bytes16_4;
	}

	/// <summary>
	/// 64 Byte structure.
	/// </summary>
	public struct TestState64_3
	{
		public Quaternion Bytes16_1;
		public Quaternion Bytes16_2;
		public Quaternion Bytes16_3;
		public Quaternion Bytes16_4;
	}
}
