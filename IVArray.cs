using System.Runtime.InteropServices;

namespace VCollectionObjects
{
	/// <summary>
	/// Provides the interface for VArray class objects.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[ComImport]
	[Guid("C0C96235-68E3-4776-B6DE-CAC104571616")]
	public interface IVArray<T>
	{
		/// <inheritdoc cref="VEnumerable{T}.Add(T)"/>
		public void Add(T value);
		/// <inheritdoc cref="VEnumerable{T}.Remove(T)"/>
		public void Remove(T value);
		/// <inheritdoc cref="VEnumerable{T}.Clear()"/>
		public void Clear();

	}
}