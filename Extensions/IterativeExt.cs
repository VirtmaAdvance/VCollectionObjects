namespace VCollectionObjects.Extensions
{
	/// <summary>
	/// Provides iterative extension methods for objects inheriting from the <see cref="IEnumerable{T}"/> interface.
	/// </summary>
	public static class IterativeExt
	{
		/// <summary>
		/// Iterates through the entire <paramref name="source"/> and returns the converted result from the operation.
		/// </summary>
		/// <typeparam name="TIn"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="source"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static IEnumerable<TOut> ForEach<TIn, TOut>(this IEnumerable<TIn> source, Func<TIn, TOut> predicate)
		{
			TOut[] res=[];
			foreach(var sel in source)
				res=res.Push(predicate(sel));
			return res;
		}

	}
}
