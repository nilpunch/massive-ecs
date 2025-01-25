using System;

namespace Massive
{
	public static class ErrorMessage
	{
		public const string ConflictingFilter = "Conflicting included and excluded components!";
		public const string ViewsWithEmptyTypes = "Don't use empty types as generic arguments in ForEach(ref T ...) methods";

		public static string EntityDead(Entity entity) => $"The {entity} is not alive.";
		public static string EntityDead(int entityId) => $"The entity with id:{entityId} is not alive.";
		public static string NotAssigned(int id) => $"The id:{id} is not assigned.";
		public static string InvalidIndex(int index) => $"The index:{index} is invalid.";

		public static string TypeHasNoData<T>(string suggestion) => $"The type {typeof(T).GetFullGenericName()} has no associated data! {suggestion}, or enable {nameof(RegistryConfig.StoreEmptyTypesAsDataSets)} in registry config.";
	}
}
