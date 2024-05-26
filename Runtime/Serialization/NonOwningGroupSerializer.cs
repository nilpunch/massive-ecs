using System.IO;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class NonOwningGroupSerializer<TInclude, TExclude> : IRegistrySerializer
		where TInclude : struct, IReadOnlySetSelector where TExclude : struct, IReadOnlySetSelector
	{
		public void Serialize(IRegistry registry, Stream stream)
		{
			var nonOwningGroup = (NonOwningGroup)registry.Group<None, TInclude, TExclude>();

			var set = (SparseSet)nonOwningGroup.GroupSet;

			SparseSetSerializer.Serialize(set, stream);
		}

		public void Deserialize(IRegistry registry, Stream stream)
		{
			var nonOwningGroup = (NonOwningGroup)registry.Group<None, TInclude, TExclude>();

			var set = (SparseSet)nonOwningGroup.GroupSet;

			SparseSetSerializer.Deserialize(set, stream);
		}
	}
}
