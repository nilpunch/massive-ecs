using System;

namespace Massive
{
	/// <summary>
	/// Enables <see cref="Packing"/>.<see cref="Packing.WithHoles"/> for the set of this component.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Delegate)]
	public class StableAttribute : Attribute
	{
	}
}
