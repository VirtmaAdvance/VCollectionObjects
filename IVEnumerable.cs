using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace VCollectionObjects
{
	/// <summary>
	/// An interface for collection objects.
	/// </summary>
	[ComImport]
	[Guid("E1335B7D-DA2B-4F75-BA3A-90FAE27E1B87")]
	public interface IVEnumerable : IEnumerable, IDisposable, ISerializable, IDeserializationCallback, IComparable, ICollection
	{
		/// <summary>
		/// Gets the number of items in the collection.
		/// </summary>
		public int Length { get; }

	}
}