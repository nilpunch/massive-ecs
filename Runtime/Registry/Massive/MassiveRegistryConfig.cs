namespace Massive
{
	public class MassiveRegistryConfig : RegistryConfig
	{
		private const int DefaultMassivePageSize = 1024;

		public readonly int FramesCapacity = Constants.DefaultFramesCapacity;

		public MassiveRegistryConfig(int? framesCapacity = default, int? pageSize = DefaultMassivePageSize,
			bool? storeEmptyTypesAsDataSets = default, bool? fullStability = default, Packing? packingWhenIterating = default)
			: base(pageSize, storeEmptyTypesAsDataSets, fullStability, packingWhenIterating)
		{
			FramesCapacity = framesCapacity ?? FramesCapacity;
		}
	}
}
