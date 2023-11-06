using NUnit.Framework;

namespace Massive.Tests
{
    [TestFixture]
    public class WorldStateTests
    {
        [Test]
        public void Reserve_ShouldInitializeData()
        {
            WorldState<int> worldState = new WorldState<int>(2, 4);

            worldState.Reserve(1);
            worldState.Reserve(2);
            worldState.Reserve(3);

            Assert.AreEqual(worldState.Get(0), 1);
            Assert.AreEqual(worldState.Get(1), 2);
            Assert.AreEqual(worldState.Get(2), 3);
        }

        [Test]
        public void Reserve_ShouldReturnCorrectHandle()
        {
            WorldState<int> worldState = new WorldState<int>(2, 4);

            var handle1 = worldState.Reserve(1);
            var handle2 = worldState.Reserve(2);
            var handle3 = worldState.Reserve(3);

            Assert.AreEqual(handle1.State, 1);
            Assert.AreEqual(handle2.State, 2);
            Assert.AreEqual(handle3.State, 3);
        }

        [Test]
        public void StateHandle_WhenAffected_ShouldChangeState()
        {
            WorldState<int> worldState = new WorldState<int>(2, 2);
            var handle1 = worldState.Reserve(1);

            handle1.State = 2;

            Assert.AreEqual(handle1.State, 2);
        }

        [Test]
        public void SaveFrame_ShouldPreserveHandles()
        {
            WorldState<int> worldState = new WorldState<int>(2, 4);

            var handle1 = worldState.Reserve(1);
            var handle2 = worldState.Reserve(2);
            var handle3 = worldState.Reserve(3);

            worldState.SaveFrame();

            Assert.AreEqual(handle1.State, 1);
            Assert.AreEqual(handle2.State, 2);
            Assert.AreEqual(handle3.State, 3);
        }

        [Test]
        public void RollbackZero_ShouldResetCurrentFrameChanges()
        {
            WorldState<int> worldState = new WorldState<int>(2, 2);

            var handle1 = worldState.Reserve(1);
            worldState.SaveFrame();

            handle1.State = 2;
            worldState.Rollback(0);

            Assert.AreEqual(handle1.State, 1);
        }
    }
}
