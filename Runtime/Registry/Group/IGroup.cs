namespace Massive
{
	public interface IGroup
	{
		int Length { get; }

		void EnsureSynced();
	}
}