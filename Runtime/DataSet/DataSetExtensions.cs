namespace Massive
{
	public static class DataSetExtensions
	{
		public static void Assign<T>(this IDataSet<T> dataSet, int id, T data = default)
		{
			if (id < 0)
			{
				return;
			}

			dataSet.Assign(id);
			dataSet.Get(id) = data;
		}

		public static void AssignCopy<T>(this IDataSet<T> dataSet, int id, T source = default) where T : ICopyable<T>
		{
			if (id < 0)
			{
				return;
			}

			dataSet.Assign(id);
			source.CopyTo(ref dataSet.Get(id));
		}
	}
}
