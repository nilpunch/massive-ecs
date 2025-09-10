using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public enum DataAccessContext
	{
		View,
		WorldDataSet,
		WorldGet,
		WorldSet,
	}

	public class NoDataException : MassiveException
	{
		private NoDataException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfHasNoData<T>(World world, DataAccessContext context)
		{
			if (!(world.SparseSet<T>() is DataSet<T>))
			{
				var suggestion = context switch
				{
					DataAccessContext.View => "Don't use empty components as generic arguments in ForEach(ref T ...) methods",
					DataAccessContext.WorldDataSet => "Use " + nameof(WorldSetExtensions.SparseSet) + "<T>() method for empty components instead",
					DataAccessContext.WorldGet => "Don't use" + nameof(WorldIdExtensions.Get) + "<T>() method with empty components",
					DataAccessContext.WorldSet => "Don't use" + nameof(WorldIdExtensions.Set) + "<T>() method with empty components",
					_ => throw new ArgumentOutOfRangeException(nameof(context), context, null)
				};

				throw new NoDataException($"The component {typeof(T).GetFullGenericName()} has no associated data! " +
					$"{suggestion}, or enable {nameof(WorldConfig.StoreEmptyTypesAsDataSets)} in world config.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfHasNoData(SparseSet sparseSet, Type type, DataAccessContext context)
		{
			if (!(sparseSet is IDataSet))
			{
				var suggestion = context switch
				{
					DataAccessContext.View => "Don't use empty components as generic arguments in ForEach(ref T ...) methods",
					DataAccessContext.WorldDataSet => "Use " + nameof(WorldSetExtensions.SparseSet) + "<T>() method for empty components instead",
					DataAccessContext.WorldGet => "Don't use" + nameof(WorldIdExtensions.Get) + "<T>() method with empty components",
					DataAccessContext.WorldSet => "Don't use" + nameof(WorldIdExtensions.Set) + "<T>() method with empty components",
					_ => throw new ArgumentOutOfRangeException(nameof(context), context, null)
				};

				throw new NoDataException($"The component {type.GetFullGenericName()} has no associated data! " +
					$"{suggestion}, or enable {nameof(WorldConfig.StoreEmptyTypesAsDataSets)} in world config.");
			}
		}
	}
}
