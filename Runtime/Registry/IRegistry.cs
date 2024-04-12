using System;

namespace Massive
{
	public interface IRegistry
	{
		IGroupsController Groups { get; }
		Entities Entities { get; }

		event Action<ISet> SetCreated;

		IDataSet<T> Components<T>() where T : struct;

		ISet Any<T>() where T : struct;
	}
}