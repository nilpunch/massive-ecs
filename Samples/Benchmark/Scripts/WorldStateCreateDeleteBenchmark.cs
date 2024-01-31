using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateCreateDeleteBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private MassiveData<TestState> _massiveData;

		private void Start()
		{
			_massiveData = new MassiveData<TestState>(100, _worldEntitiesCount);
		}

		protected override void Sample()
		{
			Frame<TestState> frame = _massiveData.CurrentFrame;

			for (int index = 0; index < _worldEntitiesCount; index++)
			{
				frame.Create(new TestState() { Value = index + 1 });
			}

			for (int index = 0; index < _worldEntitiesCount; index++)
			{
				frame.Delete(index);
			}
		}
	}
}