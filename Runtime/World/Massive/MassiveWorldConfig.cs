namespace Massive
{
	public class MassiveWorldConfig : WorldConfig
	{
		private const int DefaultMassivePageSize = 1024;

		public readonly int FramesCapacity = Constants.DefaultFramesCapacity;

		public MassiveWorldConfig(int? framesCapacity = default, int? pageSize = DefaultMassivePageSize,
			bool? storeEmptyTypesAsDataSets = default, bool? fullStability = default, Packing? packingWhenIterating = default)
			: base(pageSize, storeEmptyTypesAsDataSets, fullStability, packingWhenIterating)
		{
			FramesCapacity = framesCapacity ?? FramesCapacity;
		}
	}
}
