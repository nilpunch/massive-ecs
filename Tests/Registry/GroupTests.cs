using System;
using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class GroupTests
	{
		public enum TestGroupType
		{
			FullOwningGroup,
			PartialOwningGroup,
			NonOwningGroup,
		}

		[TestCase(TestGroupType.FullOwningGroup)]
		[TestCase(TestGroupType.PartialOwningGroup)]
		[TestCase(TestGroupType.NonOwningGroup)]
		public void Group_Functionality(TestGroupType testGroupType)
		{
			var registry = new Registry();

			var group = SetUpGroupWithIntAndChar(registry, testGroupType);

			Assert.AreEqual(0, group.Count);

			var entity1 = registry.CreateEntity('1').Id;
			var entity2 = registry.CreateEntity('2').Id;

			registry.Assign(entity2, 42);

			Assert.AreEqual(1, group.Count);

			registry.Assign<int>(entity1);

			Assert.AreEqual(2, group.Count);

			registry.Unassign<int>(entity1);

			Assert.AreEqual(1, group.Count);

			foreach (var id in group.AsIds())
			{
				Assert.AreEqual(registry.Get<int>(id), 42);
				Assert.AreEqual(registry.Get<char>(id), '2');
			}

			Assert.AreEqual(group.AsIds()[0], entity2);

			registry.Unassign<char>(entity1);
			registry.Unassign<char>(entity2);

			Assert.AreEqual(0, group.Count);
		}

		[TestCase(TestGroupType.FullOwningGroup)]
		[TestCase(TestGroupType.NonOwningGroup)]
		public void Group_Exclude(TestGroupType testGroupType)
		{
			var registry = new Registry();

			var entity1 = registry.CreateEntity(1).Id;

			var entity2 = registry.CreateEntity(2).Id;
			registry.Assign<char>(entity2);

			var group = SetUpGroupWithIntAndWithoutChar(registry, testGroupType);

			var entity3 = registry.CreateEntity(3).Id;

			var entity4 = registry.CreateEntity(4).Id;
			registry.Assign<char>(entity4);

			foreach (var entity in group.AsIds())
			{
				Assert.True(entity == entity1 || entity == entity3);

				if (entity == entity1)
				{
					Assert.AreEqual(1, registry.Get<int>(entity1));
				}
				else if (entity == entity3)
				{
					Assert.AreEqual(3, registry.Get<int>(entity3));
				}
			}

			registry.Assign<char>(entity1);
			registry.Assign<char>(entity3);

			Assert.AreEqual(0, group.Count);

			registry.Unassign<char>(entity2);
			registry.Unassign<char>(entity4);

			foreach (var entity in group.AsIds())
			{
				Assert.True(entity == entity2 || entity == entity4);

				if (entity == entity2)
				{
					Assert.AreEqual(2, registry.Get<int>(entity2));
				}
				else if (entity == entity4)
				{
					Assert.AreEqual(4, registry.Get<int>(entity4));
				}
			}
		}

		[TestCase(TestGroupType.FullOwningGroup)]
		[TestCase(TestGroupType.NonOwningGroup)]
		public void Group_StableLateInitialization(TestGroupType testGroupType)
		{
			var registry = new Registry();

			for (int i = 0; i < 30; i++)
			{
				var entity = registry.CreateEntity();

				if (i % 2 != 0)
				{
					registry.Assign<int>(entity);
				}

				if (i % 3 != 0)
				{
					registry.Assign<char>(entity);
				}
			}

			var group = SetUpGroupWithIntAndWithoutChar(registry, testGroupType);

			Assert.AreEqual(5, group.Count);
		}

		[Test]
		public void Group_ShouldAllowMultilayerNesting()
		{
			var registry = new Registry();

			var entity1 = registry.CreateEntity(1).Id;
			registry.Assign<float>(entity1);

			var entity2 = registry.CreateEntity(2).Id;
			registry.Assign<char>(entity2);
			registry.Assign<float>(entity2);

			var owningGroup3 = registry.Group<Include<float, double>, None, Own<int, char>>();
			var owningGroup = registry.Group<Include<float>, None, Own<int>>();
			var owningGroup2 = registry.Group<Include<float>, None, Own<int, char>>();

			var entity3 = registry.CreateEntity(3).Id;
			registry.Assign<float>(entity3);

			var entity4 = registry.CreateEntity(4).Id;
			registry.Assign<char>(entity4);
			registry.Assign<float>(entity4);

			Assert.AreEqual(4, owningGroup.Count);
			Assert.AreEqual(2, owningGroup2.Count);
			Assert.AreEqual(0, owningGroup3.Count);

			registry.Assign<char>(entity1);
			registry.Assign<char>(entity3);

			Assert.AreEqual(4, owningGroup.Count);
			Assert.AreEqual(4, owningGroup2.Count);
			Assert.AreEqual(0, owningGroup3.Count);

			registry.Assign<double>(entity1);
			registry.Assign<double>(entity3);

			Assert.AreEqual(4, owningGroup.Count);
			Assert.AreEqual(4, owningGroup2.Count);
			Assert.AreEqual(2, owningGroup3.Count);

			registry.Unassign<char>(entity1);
			registry.Unassign<char>(entity3);

			Assert.AreEqual(4, owningGroup.Count);
			Assert.AreEqual(2, owningGroup2.Count);
			Assert.AreEqual(0, owningGroup3.Count);

			registry.Unassign<float>(entity2);
			registry.Unassign<float>(entity4);

			Assert.AreEqual(2, owningGroup.Count);
			Assert.AreEqual(0, owningGroup2.Count);
			Assert.AreEqual(0, owningGroup3.Count);
		}

		private IGroup SetUpGroupWithIntAndChar(Registry registry, TestGroupType testGroupType)
		{
			switch (testGroupType)
			{
				case TestGroupType.FullOwningGroup:
					return registry.Group<None, None, Own<int, char>>();
				case TestGroupType.PartialOwningGroup:
					return registry.Group<Include<char>, None, Own<int>>();
				case TestGroupType.NonOwningGroup:
					return registry.Group<Include<int, char>>();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private IGroup SetUpGroupWithIntAndWithoutChar(Registry registry, TestGroupType testGroupType)
		{
			switch (testGroupType)
			{
				case TestGroupType.FullOwningGroup:
				case TestGroupType.PartialOwningGroup:
					return registry.Group<None, Exclude<char>, Own<int>>();
				case TestGroupType.NonOwningGroup:
					return registry.Group<Include<int>, Exclude<char>>();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
