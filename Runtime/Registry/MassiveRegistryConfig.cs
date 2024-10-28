namespace Massive
{
	public class MassiveRegistryConfig : RegistryConfig
	{
		public int FramesCapacity = Constants.DefaultFramesCapacity;

		public MassiveRegistryConfig() { DataPageSize = 1024; }
	}
}
