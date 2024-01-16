using System.Runtime.InteropServices;

namespace VCollectionObjects
{
	/// <summary>
	/// An interface for the VCollection class.
	/// </summary>
	[ComImport]
	[Guid("824DC536-1CB8-4120-8547-BA7E9412C34D")]
	public interface IVCollection<TKey, TValue>
	{
		/// <inheritdoc cref="VEnumerable{T}.Add(T)"/>
		public void Add(TKey key, TValue value);
		/// <inheritdoc cref="VEnumerable{T}.Add(T)"/>
		public void Add(KeyValuePair<TKey, TValue> item);
		/// <inheritdoc cref="VEnumerable{T}.Contains(T)"/>
		public bool ContainsKey(TKey key);
		/// <inheritdoc cref="VEnumerable{T}.Contains(T)"/>
		public bool ContainsValue(TValue value);
		/// <inheritdoc cref="VEnumerable{T}.Remove(T)"/>
		public void Remove(TKey key);
		/// <inheritdoc cref="VEnumerable{T}.Clear()"/>
		public void Clear();

	}
}