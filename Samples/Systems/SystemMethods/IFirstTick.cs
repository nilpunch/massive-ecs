namespace Massive.Samples.UpdateLoop
{
	public interface IFirstTick : ISystemMethod<IFirstTick>
	{
		void FirstTick();

		void ISystemMethod<IFirstTick>.Run() => FirstTick();
	}
}
