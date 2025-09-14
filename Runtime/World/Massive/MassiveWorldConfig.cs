namespace Massive
{
	public class MassiveWorldConfig : WorldConfig
	{
		private const int DefaultMassivePageSize = 256;

		public readonly int FramesCapacity = Constants.DefaultFramesCapacity;

		public MassiveWorldConfig(int? framesCapacity = default, int? pageSize = DefaultMassivePageSize, bool? storeEmptyTypesAsDataSets = default)
			: base(pageSize, storeEmptyTypesAsDataSets)
		{
			FramesCapacity = framesCapacity ?? FramesCapacity;
		}
	}
}
