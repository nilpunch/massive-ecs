using System;

namespace Massive
{
	/// <summary>
	/// Stores set of entities that does not have component of this type.
	/// Speeds up exclusion filtering at the cost of extra memory and slower entity creation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Delegate)]
	public class StoreNegativeAttribute : Attribute
	{
	}
}
