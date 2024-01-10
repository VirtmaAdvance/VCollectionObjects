using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCollectionObjects
{
	public interface IVEnumerable<T> : IEnumerable<T?>
	{

		/// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool}, TSource)"/>
		public T? FirstOrDefault(Func<T?, bool> predicate, T? defaultValue);
		/// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool}, TSource)"/>
		public T? FirstOrDefault(Func<T?, bool> predicate);
		/// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool}, TSource)"/>
		public T? FirstOrDefault();
		/// <inheritdoc cref="Enumerable.First{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
		public T? First(Func<T?, bool> predicate);
		/// <inheritdoc cref="Enumerable.First{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
		public T? First();
		/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, int, TResult})"/>
		public IEnumerable<TResult> Select<TResult>(Func<T? , int, TResult> predicate);
		/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, int, TResult})"/>
		public IEnumerable<TResult> Select<TResult>(Func<T?, TResult> predicate);
		/// <inheritdoc cref="Enumerable.SelectMany{TSource, TResult}(IEnumerable{TSource}, Func{TSource, int, IEnumerable{TResult}})"/>
		public IEnumerable<TResult> SelectMany<TResult>(Func<T?, int, IEnumerable<TResult>> predicate);
		/// <inheritdoc cref="Enumerable.SelectMany{TSource, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TResult}})"/>
		public IEnumerable<TResult> SelectMany<TResult>(Func<T?, IEnumerable<TResult>> predicate);
		/// <inheritdoc cref="Enumerable.SelectMany{TSource, TCollection, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TCollection}}, Func{TSource, TCollection, TResult})"/>
		public IEnumerable<TResult> SelectMany<TCollection, TResult>(Func<T?, IEnumerable<TCollection>> collectionSelector, Func<T?, TCollection, TResult> predicate);
		/// <inheritdoc cref="Enumerable.SelectMany{TSource, TCollection, TResult}(IEnumerable{TSource}, Func{TSource, int, IEnumerable{TCollection}}, Func{TSource, TCollection, TResult})"/>
		public IEnumerable<TResult> SelectMany<TCollection, TResult>(Func<T?, int, IEnumerable<TCollection>> collectionSelector, Func<T?, TCollection, TResult> predicate);
		/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, int, bool})"/>
		public IEnumerable<T?> Where(Func<T?, int, bool> predicate);
		/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
		public IEnumerable<T?> Where(Func<T?, bool> predicate);


	}
}
