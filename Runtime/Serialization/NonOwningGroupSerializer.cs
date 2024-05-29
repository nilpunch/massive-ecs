using System.IO;

namespace Massive.Serialization
{
	public class NonOwningGroupSerializer<TInclude, TExclude> : IRegistrySerializer
		where TInclude : struct, IIncludeSelector
		where TExclude : struct, IExcludeSelector
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
