using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class EntityIdentifiersTests
	{
		// ReSharper disable RedundantArgumentDefaultValue
		private const int Capacity = 100;

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void IsAlive_WhenNotCreated_ShouldBeFalse(int id)
		{
			var entities = new Entities(Capacity);

			var isAlive = entities.IsAlive(id);

			Assert.IsFalse(isAlive);
		}

		[TestCase(200)]
		[TestCase(-1)]
		public void IsAlive_WhenOutOfBounds_ShouldNotThrow(int id)
		{
			var entities = new Entities(Capacity);

			Assert.DoesNotThrow(CheckAlive);

			void CheckAlive() => entities.IsAlive(id);
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void Create_ShouldMakeAlive(int id)
		{
			var entities = new Entities(Capacity);
			for (var i = 0; i <= id; i++)
				entities.Create();

			var isAlive = entities.IsAlive(id);

			Assert.IsTrue(isAlive);
		}

		[TestCase(Capacity + 1)]
		public void Create_WhenOutOfBounds_ShouldResize(int id)
		{
			var entities = new Entities(Capacity);

			Assert.DoesNotThrow(CreateCheck);

			void CreateCheck()
			{
				for (var i = 0; i <= id; i++)
					entities.Create();
			}
		}

		[TestCase(10)]
		public void Create_ShouldGenerateDistinctIds(int createAmount)
		{
			var entities = new Entities(Capacity);
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
			var entities = new Entities(Capacity);
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
			var entities = new Entities(Capacity);

			Assert.DoesNotThrow(CheckDestroy);

			void CheckDestroy() => entities.Destroy(id);
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void DestroyThenCreate_ShouldRecycleIds(int id)
		{
			var entities = new Entities(Capacity);
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
			var entities = new Entities(Capacity);

			entities.CreateMany(id + 1);
			var isAlive = entities.IsAlive(id);

			Assert.IsTrue(isAlive);
		}

		[TestCase(Capacity + 1)]
		public void CreateMany_WhenOutOfBounds_ShouldResize(int id)
		{
			var entities = new Entities(Capacity);

			Assert.DoesNotThrow(CreateManyCheck);

			void CreateManyCheck()
			{
				entities.CreateMany(id);
			}
		}

		[TestCase(10)]
		public void CreateMany_ShouldGenerateDistinctIds(int createAmount)
		{
			var entities = new Entities(Capacity);
			var created = new List<Entity>();

			entities.CreateMany(createAmount, created.Add);
			var distinctIds = created.Distinct().Count();

			Assert.AreEqual(createAmount, distinctIds);
		}
	}
}