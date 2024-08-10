using System.IO;

namespace Massive.Serialization
{
	public class NonOwningGroupSerializer<TInclude, TExclude> : IRegistrySerializer
		where TInclude : IIncludeSelector, new()
		where TExclude : IExcludeSelector, new()
	{
		public void Serialize(Registry registry, Stream stream)
		{
			var nonOwningGroup = (NonOwningGroup)registry.Group<None, TInclude, TExclude>();

			var set = nonOwningGroup.GroupSet;

			SparseSetSerializer.Serialize(set, stream);
		}

		public void Deserialize(Registry registry, Stream stream)
		{
			var nonOwningGroup = (NonOwningGroup)registry.Group<None, TInclude, TExclude>();

			var set = nonOwningGroup.GroupSet;

			SparseSetSerializer.Deserialize(set, stream);
		}
	}
}
