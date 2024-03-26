namespace Massive
{
	public interface IFilter
	{
		ISet[] Include { get; }
		ISet[] Exclude { get; }
		
		bool Contains(int id);
	}
}