using System;
using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture(TestGroupType.FullOwningGroup)]
	[TestFixture(TestGroupType.PartialOwningGroup)]
	[TestFixture(TestGroupType.NonOwningGroup)]
	public class GroupsTests
	{
		public enum TestGroupType
		{
			FullOwningGroup,
			PartialOwningGroup,
			NonOwningGroup,
		}

		private readonly TestGroupType _testGroupType;

		public GroupsTests(TestGroupType testGroupType)
		{
			_testGroupType = testGroupType;
		}

		[Test]
		public void Group_FunctionalityTest()
		{
			var registry = new Registry();

			var group = SetUpGroupWithIntAndChar(registry);

			Assert.AreEqual(group.Length, 0);

			var entity1 = registry.Create('1');
			var entity2 = registry.Create('2');

			registry.Add(entity2, 42);

			Assert.AreEqual(group.Length, 1);

			registry.Add<int>(entity1);

			Assert.AreEqual(group.Length, 2);

			registry.Remove<int>(entity1);

			Assert.AreEqual(group.Length, 1);

			foreach (var id in group.Ids)
			{
				Assert.AreEqual(registry.Get<int>(id), 42);
				Assert.AreEqual(registry.Get<char>(id), '2');
			}

			Assert.AreEqual(group.Ids[0], entity2);

			registry.Remove<char>(entity1);
			registry.Remove<char>(entity2);

			Assert.AreEqual(group.Length, 0);
		}

		[Test]
		public void Group_ExclusionTest()
		{
			var registry = new Registry();

			var entity1 = registry.Create(1);

			var entity2 = registry.Create(2);
			registry.Add<char>(entity2);

			var group = SetUpGroupWithIntAndWithoutChar(registry);

			var entity3 = registry.Create(3);

			var entity4 = registry.Create(4);
			registry.Add<char>(entity4);

			foreach (var entity in group.Ids)
			{
				Assert.True(entity == entity1 || entity == entity3);

				if (entity == entity1)
				{
					Assert.AreEqual(registry.Get<int>(entity1), 1);
				}
				else if (entity == entity3)
				{
					Assert.AreEqual(registry.Get<int>(entity3), 3);
				}
			}

			registry.Add<char>(entity1);
			registry.Add<char>(entity3);

			Assert.AreEqual(group.Length, 0);

			registry.Remove<char>(entity2);
			registry.Remove<char>(entity4);

			foreach (var entity in group.Ids)
			{
				Assert.True(entity == entity2 || entity == entity4);

				if (entity == entity2)
				{
					Assert.AreEqual(registry.Get<int>(entity2), 2);
				}
				else if (entity == entity4)
				{
					Assert.AreEqual(registry.Get<int>(entity4), 4);
				}
			}
		}

		private IGroup SetUpGroupWithIntAndChar(IRegistry registry)
		{
			switch (_testGroupType)
			{
				case TestGroupType.FullOwningGroup:
					return registry.Groups.EnsureGroup(owned: new ISet[]
					{
						registry.Components<int>(),
						registry.Components<char>()
					});
				case TestGroupType.PartialOwningGroup:
					return registry.Groups.EnsureGroup(
						owned: new ISet[]
						{
							registry.Components<int>()
						},
						include: new IReadOnlySet[]
						{
							registry.Components<char>()
						}
					);
				case TestGroupType.NonOwningGroup:
					return registry.Groups.EnsureGroup(include: new IReadOnlySet[]
					{
						registry.Components<int>(),
						registry.Components<char>()
					});
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private IGroup SetUpGroupWithIntAndWithoutChar(IRegistry registry)
		{
			switch (_testGroupType)
			{
				case TestGroupType.FullOwningGroup:
				case TestGroupType.PartialOwningGroup:
					return registry.Groups.EnsureGroup(
						owned: new ISet[]
						{
							registry.Components<int>(),
						},
						exclude: new IReadOnlySet[]
						{
							registry.Components<char>()
						});
				case TestGroupType.NonOwningGroup:
					return registry.Groups.EnsureGroup(
						include: new IReadOnlySet[]
						{
							registry.Components<int>(),
						},
						exclude: new IReadOnlySet[]
						{
							registry.Components<char>()
						});
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}