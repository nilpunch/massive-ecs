namespace Massive
{
	public static class SuggestionMessage
	{
		public const string DontUseViewsWithEmptyTypes = "Don't use empty types as generic arguments in ForEach(ref T ...) methods";
		public const string UseSetMethodWithEmptyTypes = "Use " + nameof(RegistrySetExtensions.Set) + "<T>() method for empty types instead";
		public const string DontUseGetWithEmptyTypes = "Don't use " + nameof(RegistryIdExtensions.Get) + "<T>() method with empty types";
	}
}
