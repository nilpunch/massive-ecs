namespace Massive
{
	public interface IFilter
	{
		IReadOnlySet[] Include { get; }
		IReadOnlySet[] Exclude { get; }

		bool ContainsId(int id);
	}
}
