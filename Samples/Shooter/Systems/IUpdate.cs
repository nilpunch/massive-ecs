namespace Massive.Samples.Shooter
{
	public interface IUpdate : ISystemMethod<IUpdate, float>
	{
		void Update(float deltaTime);

		void ISystemMethod<IUpdate, float>.Run(float arg) => Update(arg);
	}
}
