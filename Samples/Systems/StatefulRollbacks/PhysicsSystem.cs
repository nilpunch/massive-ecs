using Massive.Samples.UpdateLoop;

namespace Massive.Samples.StatefulRollbacks
{
	public class PhysicsSystem : SystemBase<PhysicsSystem.SystemState>, IFirstTick, IUpdate
	{
		public struct SystemState
		{
			public CollisionTree CollisionTree;
			public ListPointer<RigidBody> Bodies;
			public int AnyOtherData;
		}

		public void FirstTick()
		{
			State.CollisionTree.Nodes = Allocator.AllocList<CollisionTree.Node>();
			State.Bodies = Allocator.AllocList<RigidBody>();
		}

		public void Update(float deltaTime)
		{
			var bodies = State.Bodies.In(Allocator);

			foreach (ref var rigidBody in bodies)
			{
				rigidBody.PosY -= deltaTime;
			}
		}
	}
}
