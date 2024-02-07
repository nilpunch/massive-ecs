namespace MassiveData
{
	public interface IMassive
	{
		void SaveFrame();
		void Rollback(int frames);
	}
}