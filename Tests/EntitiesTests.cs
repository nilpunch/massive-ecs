using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class EntitiesTests
	{
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void IsAlive_WhenNotCreated_ShouldBeFalse(int id)
		{
			var entities = new Entities();

			var isAlive = entities.IsAlive(id);

			Assert.IsFalse(isAlive);
		}

		[TestCase(200)]
		[TestCase(-1)]
		public void IsAlive_WhenOutOfBounds_ShouldNotThrow(int id)
		{
			var entities = new Entities();

			Assert.DoesNotThrow(CheckAlive);

			void CheckAlive() => entities.IsAlive(id);
		}

		[Test]
		public void GetEntity_ShouldReturnCorrectEntity()
		{
			var entities = new Entities();

			for (var i = 0; i < 10; i++)
				entities.Create();

			Assert.AreEqual(0, entities.GetEntity(0).Id);
			Assert.AreEqual(3, entities.GetEntity(3).Id);
			Assert.AreEqual(6, entities.GetEntity(6).Id);

			for (var i = 0; i <= 5; i++)
			{
				entities.Destroy(i);
				entities.Create();
			}

			Assert.AreEqual(0, entities.GetEntity(0).Id);
			Assert.AreEqual(3, entities.GetEntity(3).Id);
			Assert.AreEqual(6, entities.GetEntity(6).Id);
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void Create_ShouldMakeAlive(int id)
		{
			var entities = new Entities();
			for (var i = 0; i <= id; i++)
				entities.Create();

			var isAlive = entities.IsAlive(id);

			Assert.IsTrue(isAlive);
		}

		[TestCase(Constants.DefaultCapacity + 10)]
		[TestCase(Constants.DefaultCapacity + 1000)]
		public void Create_WhenOutOfBounds_ShouldResize(int createAmount)
		{
			var entities = new Entities();

			Assert.DoesNotThrow(CreateCheck);

			void CreateCheck()
			{
				for (var i = 0; i <= createAmount; i++)
					entities.Create();
			}
		}

		[TestCase(10)]
		public void Create_ShouldGenerateDistinctIds(int createAmount)
		{
			var entities = new Entities();
			var created = new List<int>();
			for (var i = 0; i < createAmount; i++)
				created.Add(entities.Create().Id);

			var distinctIds = created.Distinct().Count();

			Assert.AreEqual(createAmount, distinctIds);
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void Destroy_ShouldMakeNotAlive(int id)
		{
			var entities = new Entities();
			for (var i = 0; i <= id; i++)
				entities.Create();

			entities.Destroy(id);
			var isAlive = entities.IsAlive(id);

			Assert.IsFalse(isAlive);
		}

		[TestCase(200)]
		[TestCase(-1)]
		public void Destroy_WhenOutOfBounds_ShouldNotThrow(int id)
		{
			var entities = new Entities();

			Assert.DoesNotThrow(CheckDestroy);

			void CheckDestroy() => entities.Destroy(id);
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void DestroyThenCreate_ShouldRecycleIds(int id)
		{
			var entities = new Entities();
			for (var i = 0; i <= id; i++)
				entities.Create();

			entities.Destroy(id);
			var createdIdentifier = entities.Create();

			Assert.AreEqual(id, createdIdentifier.Id);
			Assert.AreEqual(1, createdIdentifier.ReuseCount);
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void CreateMany_ShouldMakeAlive(int id)
		{
			var entities = new Entities();

			entities.CreateMany(id + 1);
			var isAlive = entities.IsAlive(id);

			Assert.IsTrue(isAlive);
		}

		[TestCase(Constants.DefaultCapacity + 10)]
		[TestCase(Constants.DefaultCapacity + 1000)]
		public void CreateMany_WhenOutOfBounds_ShouldResize(int createAmount)
		{
			var entities = new Entities();

			Assert.DoesNotThrow(CreateManyCheck);

			void CreateManyCheck()
			{
				entities.CreateMany(createAmount);
			}
		}

		[TestCase(10)]
		public void CreateMany_ShouldGenerateDistinctIds(int createAmount)
		{
			var entities = new Entities();
			var created = new List<Entity>();

			entities.CreateMany(createAmount, created.Add);
			var distinctIds = created.Distinct().Count();

			Assert.AreEqual(createAmount, distinctIds);
		}
	}
}
