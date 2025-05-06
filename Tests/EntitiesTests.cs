using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class EntitiesTests
	{
		public int EntitiesToCreate = 24;

		public int[] IdsToDestroy =
		{
			0, 1, 3, 20, 21, 23
		};

		[Test]
		public void WhenCompact_AndThereIsHoles_ThenRemoveHoles()
		{
			// Arrange.
			var entities = new Entities(Packing.WithHoles);
			for (int i = 0; i < EntitiesToCreate; i++)
				entities.Create();
			foreach (var id in IdsToDestroy)
				entities.Destroy(id);

			// Act.
			entities.Compact();

			// Assert.
			int remainIdsCount = EntitiesToCreate - IdsToDestroy.Length;
			Assert.AreEqual(remainIdsCount, entities.Count);
		}
		
		[Test]
		public void WhenClear_AndThereIsHoles_ThenRestoreTheIdsAndClear()
		{
			// Arrange.
			var entities = new Entities(Packing.WithHoles);
			for (int i = 0; i < EntitiesToCreate; i++)
				entities.Create();
			foreach (var id in IdsToDestroy)
				entities.Destroy(id);

			// Act.
			entities.Clear();

			// Assert.
			for (int i = 0; i < EntitiesToCreate; i++)
			{
				Assert.IsFalse(entities.IsAlive(i));
				Assert.IsTrue(entities.Packed[i] >= 0);
			}
		}
		
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

		[TestCase(4 + 10)]
		[TestCase(4 + 1000)]
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
			Assert.AreEqual(2, createdIdentifier.Version);
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void Clear_ShouldMakeNotAlive(int id)
		{
			var entities = new Entities();
			for (var i = 0; i <= id; i++)
				entities.Create();

			entities.Clear();
			var isAlive = entities.IsAlive(id);

			Assert.IsFalse(isAlive);
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void Clear_ShouldMakeCountZero(int id)
		{
			var entities = new Entities();
			for (var i = 0; i <= id; i++)
				entities.Create();

			entities.Clear();
			var count = entities.Count;

			Assert.AreEqual(0, count);
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void ClearThenCreate_ShouldRecycleIds(int id)
		{
			var entities = new Entities();
			for (var i = 0; i <= id; i++)
				entities.Create();

			entities.Clear();
			var createdEntity = entities.Create();

			Assert.LessOrEqual(createdEntity.Id, id);
			Assert.AreEqual(2, createdEntity.Version);
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

		[TestCase(4 + 10)]
		[TestCase(4 + 1000)]
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
			var created = new List<int>();
			entities.AfterCreated += created.Add;

			entities.CreateMany(createAmount);
			var distinctIds = created.Distinct().Count();

			Assert.AreEqual(createAmount, distinctIds);
		}

		[Test]
		public void EntityDead_ShouldAlwaysBeDead()
		{
			var entities = new Entities();

			entities.CreateMany(10);

			Assert.IsFalse(entities.IsAlive(Entity.Dead));
		}
	}
}
