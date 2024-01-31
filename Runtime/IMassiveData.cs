namespace Massive
{
	public interface IMassiveData
	{
		void SaveFrame();
		void Rollback(int frames);
	}
}