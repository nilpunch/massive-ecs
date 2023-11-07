namespace Massive.Samples.Shooter
{
    public class SimulationGroup : ISimulation
    {
        private readonly ISimulation[] _simulations;

        public SimulationGroup(ISimulation[] simulations)
        {
            _simulations = simulations;
        }

        public void StepForward()
        {
            foreach (var simulation in _simulations)
            {
                simulation.StepForward();
            }
        }
    }
}
