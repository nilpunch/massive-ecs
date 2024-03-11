using System.Diagnostics.Contracts;

namespace Massive
{
	public interface ISetFactory
	{
		[Pure]
		ISet CreateSet();

		[Pure]
		ISet CreateDataSet<T>() where T : struct;

		[Pure]
		Identifiers CreateIdentifiers();
	}
}