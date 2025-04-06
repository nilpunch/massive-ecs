namespace Massive
{
	public static class SuggestionMessage
	{
		public const string DontUseViewsWithEmptyTypes = "Don't use empty types as generic arguments in ForEach(ref T ...) methods";
		public const string UseSparseSetMethodWithEmptyTypes = "Use " + nameof(WorldSetExtensions.SparseSet) + "<T>() method for empty types instead";
	}
}
