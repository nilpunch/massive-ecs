using System.IO;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class NonOwningGroupParser : IRegistryParser
	{
		private readonly ComponentsGroup _include;
		private readonly ComponentsGroup _exclude;

		public NonOwningGroupParser(ComponentsGroup include = null, ComponentsGroup exclude = null)
		{
			_include = include ?? new ComponentsGroup();
			_exclude = exclude ?? new ComponentsGroup();
		}

		public void Write(IRegistry registry, Stream stream)
		{
			var set = ((NonOwningGroup)registry.Group(include: _include.GetMany(registry), exclude: _exclude.GetMany(registry))).GroupSet;

			SparseSetParser.Write(set, stream);
		}

		public void Read(IRegistry registry, Stream stream)
		{
			var set = ((NonOwningGroup)registry.Group(include: _include.GetMany(registry), exclude: _exclude.GetMany(registry))).GroupSet;

			SparseSetParser.Read(set, stream);
		}
	}
}
