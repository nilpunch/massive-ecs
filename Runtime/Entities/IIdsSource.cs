namespace Massive
{
	public interface IIdsSource
	{
		int Count { get; }

		int[] Ids { get; }
	}
}
