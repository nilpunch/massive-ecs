namespace Massive
{
	public interface IDataSet : IIdsSource
	{
		void CopyData(int sourceId, int targetId);
	}
}
