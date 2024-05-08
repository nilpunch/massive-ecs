using System.IO;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class NonOwningGroupSerializer : IRegistrySerializer
	{
		private readonly SetSelector _includeSets;
		private readonly SetSelector _excludeSets;

		public NonOwningGroupSerializer(SetSelector includeSets = null, SetSelector excludeSets = null)
		{
			_includeSets = includeSets ?? Select.Nothing;
			_excludeSets = excludeSets ?? Select.Nothing;
		}

		public void Serialize(IRegistry registry, Stream stream)
		{
			var nonOwningGroup = ((NonOwningGroup)registry.Group(include: _includeSets(registry), exclude: _excludeSets(registry)));

			var set = (SparseSet)nonOwningGroup.GroupSet;

			SparseSetSerializer.Serialize(set, stream);
		}

		public void Deserialize(IRegistry registry, Stream stream)
		{
			var nonOwningGroup = ((NonOwningGroup)registry.Group(include: _includeSets(registry), exclude: _excludeSets(registry)));

			var set = (SparseSet)nonOwningGroup.GroupSet;

			SparseSetSerializer.Deserialize(set, stream);
		}
	}
}
