using System.Collections;
using System.Runtime.Serialization;

namespace VCollectionObjects
{
	public interface IVEnumerable : IEnumerable, IDisposable, ISerializable, IDeserializationCallback, IComparable
	{

		public int Length { get; }

	}
}