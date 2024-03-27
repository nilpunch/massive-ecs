namespace Massive
{
	public interface IFilter
	{
		IReadOnlySet[] Include { get; }
		IReadOnlySet[] Exclude { get; }

		bool Contains(int id);
	}
}