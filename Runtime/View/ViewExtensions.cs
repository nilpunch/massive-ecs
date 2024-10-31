using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class ViewExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView>(this TView view, EntityAction action)
			where TView : IView
		{
			var entityActionAdapter = new EntityActionAdapter { Action = action };
			view.ForEach(ref entityActionAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra>(this TView view, TExtra extra, EntityActionExtra<TExtra> action)
			where TView : IView
		{
			var entityActionExtraAdapter = new EntityActionExtraAdapter<TExtra>() { Action = action, Extra = extra };
			view.ForEach(ref entityActionExtraAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Fill<TView>(this TView view, IList<int> result)
			where TView : IView
		{
			var fillIds = new FillIds { Result = result };
			view.ForEach(ref fillIds);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Fill<TView>(this TView view, IList<Entity> result)
			where TView : IView
		{
			var fillEntities = new FillEntities { Result = result, Entities = view.Registry.Entities };
			view.ForEach(ref fillEntities);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int First<TView>(this TView view)
			where TView : IView
		{
			var returnFirst = new ReturnFirst() { Result = Constants.InvalidId };
			view.ForEach(ref returnFirst);
			return returnFirst.Result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity FirstEntity<TView>(this TView view)
			where TView : IView
		{
			var returnFirstEntity = new ReturnFirstEntity { Result = Entity.Dead, Entities = view.Registry.Entities };
			view.ForEach(ref returnFirstEntity);
			return returnFirstEntity.Result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Destroy<TView>(this TView view)
			where TView : IView
		{
			var destroyEntities = new DestroyAll { Entities = view.Registry.Entities };
			view.ForEach(ref destroyEntities);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Count<TView>(this TView view)
			where TView : IView
		{
			var countEntities = new CountAll();
			view.ForEach(ref countEntities);
			return countEntities.Result;
		}
	}
}
