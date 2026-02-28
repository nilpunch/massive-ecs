namespace Massive
{
	public class MassiveWorldConfig : WorldConfig
	{
		public readonly int FramesCapacity = Constants.DefaultFramesCapacity;

		public MassiveWorldConfig(int? framesCapacity = default, bool? storeEmptyTypesAsDataSets = default)
			: base(storeEmptyTypesAsDataSets)
		{
			FramesCapacity = framesCapacity ?? FramesCapacity;
		}

		public new MassiveWorldConfig ExcludeImplicitly<T>()
		{
			ExcludedImplicitly.Add(new Selector<T>());
			return this;
		}
	}
}
