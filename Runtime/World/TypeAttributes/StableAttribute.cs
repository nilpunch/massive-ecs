using System;

namespace Massive
{
	/// <summary>
	/// Enables <see cref="Packing"/>.<see cref="Packing.WithHoles"/> for the set of this type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Delegate)]
	public class StableAttribute : Attribute
	{
	}
}
