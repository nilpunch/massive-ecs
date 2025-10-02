namespace Massive.Samples.UpdateLoop
{
	public interface IInitinalize : ISystemMethod<IInitinalize>
	{
		void Initialize();

		void ISystemMethod<IInitinalize>.Run() => Initialize();
	}
}
