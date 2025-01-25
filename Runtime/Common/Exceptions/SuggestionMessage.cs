namespace Massive
{
	public static class SuggestionMessage
	{
		public const string ViewsWithEmptyTypes = "Don't use empty types as generic arguments in ForEach(ref T ...) methods";
		public const string UseSetMethod = "Use " + nameof(RegistrySetExtensions.Set) + "<T>() method for empty types instead";
		public const string DontUseGet = "Don't use " + nameof(RegistryIdExtensions.Get) + "<T>() method with empty types";
	}
}
