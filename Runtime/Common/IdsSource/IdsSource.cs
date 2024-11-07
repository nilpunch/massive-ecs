namespace Massive
{
	public abstract class IdsSource
	{
		public int Count { get; set; }

		public int[] Ids { get; protected set; }

		public abstract PackingMode PackingMode { get; set; }
	}
}
