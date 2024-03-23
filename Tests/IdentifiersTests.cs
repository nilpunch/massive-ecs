using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class IdentifiersTests
	{
		// ReSharper disable RedundantArgumentDefaultValue
		private const int Capacity = 100;

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void IsAlive_WhenNotCreated_ShouldBeFalse(int id)
		{
			var identifiers = new Identifiers(Capacity);

			var isAlive = identifiers.IsAlive(id);

			Assert.IsFalse(isAlive);
		}

		[TestCase(200)]
		[TestCase(-1)]
		public void IsAlive_WhenOutOfBounds_ShouldNotThrow(int id)
		{
			var identifiers = new Identifiers(Capacity);

			Assert.DoesNotThrow(CheckAlive);

			void CheckAlive() => identifiers.IsAlive(id);
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void Create_ShouldMakeAlive(int id)
		{
			var identifiers = new Identifiers(Capacity);
			for (var i = 0; i <= id; i++)
				identifiers.Create();

			var isAlive = identifiers.IsAlive(id);

			Assert.IsTrue(isAlive);
		}

		[TestCase(Capacity + 1)]
		public void Create_WhenOutOfBounds_ShouldThrow(int id)
		{
			var identifiers = new Identifiers(Capacity);

			Assert.Throws<InvalidOperationException>(CreateCheck);

			void CreateCheck()
			{
				for (var i = 0; i <= id; i++)
					identifiers.Create();
			}
		}

		[TestCase(10)]
		public void Create_ShouldGenerateDistinctIds(int createAmount)
		{
			var identifiers = new Identifiers(Capacity);
			var created = new List<int>();
			for (var i = 0; i < createAmount; i++)
				created.Add(identifiers.Create());

			var distinctIds = created.Distinct().Count();

			Assert.AreEqual(distinctIds, createAmount);
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void Delete_ShouldMakeNotAlive(int id)
		{
			var identifiers = new Identifiers(Capacity);
			for (var i = 0; i <= id; i++)
				identifiers.Create();

			identifiers.Delete(id);
			var isAlive = identifiers.IsAlive(id);

			Assert.IsFalse(isAlive);
		}

		[TestCase(200)]
		[TestCase(-1)]
		public void Delete_WhenOutOfBounds_ShouldNotThrow(int id)
		{
			var identifiers = new Identifiers(Capacity);

			Assert.DoesNotThrow(CheckDelete);

			void CheckDelete() => identifiers.Delete(id);
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void DeleteThenCreate_ShouldRecycleIds(int id)
		{
			var identifiers = new Identifiers(Capacity);
			for (var i = 0; i <= id; i++)
				identifiers.Create();

			identifiers.Delete(id);
			int createdId = identifiers.Create();

			Assert.AreEqual(createdId, id);
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void CreateMany_ShouldMakeAlive(int id)
		{
			var identifiers = new Identifiers(Capacity);

			identifiers.CreateMany(id + 1);
			var isAlive = identifiers.IsAlive(id);

			Assert.IsTrue(isAlive);
		}

		[TestCase(Capacity + 1)]
		public void CreateMany_WhenOutOfBounds_ShouldThrow(int id)
		{
			var identifiers = new Identifiers(Capacity);

			Assert.Throws<InvalidOperationException>(CreateManyCheck);

			void CreateManyCheck()
			{
				identifiers.CreateMany(id);
			}
		}

		[TestCase(10)]
		public void CreateMany_ShouldGenerateDistinctIds(int createAmount)
		{
			var identifiers = new Identifiers(Capacity);
			var created = new List<int>();

			identifiers.CreateMany(createAmount, created.Add);
			var distinctIds = created.Distinct().Count();

			Assert.AreEqual(distinctIds, createAmount);
		}
	}
}