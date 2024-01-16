using System.Runtime.InteropServices;

namespace VCollectionObjects
{
	/// <summary>
	/// Provides access flags for collections.
	/// </summary>
	[Guid("59BFAFD3-225C-4D62-9563-19F51BF2933D")]
	[Flags]
	public enum CollectionAccessStateFlags : int
	{
		/// <summary>
		/// No adjustments can be made.
		/// </summary>
		Locked=0,
		/// <summary>
		/// Allowed to read the collection and it's contents (This is the equivalence of a read only collection).
		/// </summary>
		Read=1,
		/// <summary>
		/// Allows the ability to index the collection to obtain a value.
		/// </summary>
		IndexRead=2,
		/// <summary>
		/// Allowed to add a new item or items to the collection.
		/// </summary>
		Add=4,
		/// <summary>
		/// Allowed to remove an item or items from the collection.
		/// </summary>
		Remove=8,
		/// <summary>
		/// Allows the collection to be cleared via the <see cref="VEnumerable{T}.Clear()"/> method.
		/// </summary>
		Clearable=16,
		/// <summary>
		/// Allows the replacement of an existing value within the collection.
		/// </summary>
		Replace=32,
		/// <summary>
		/// Allowed to move items within the collection (Includes replacing other items).
		/// </summary>
		Move=64,
		/// <summary>
		/// Same as <see cref="Move"/>, but will adjust the size of the collection afterwards (Involving the removal of items within the collection).
		/// </summary>
		Shift=128,
		/// <summary>
		/// Allows the collection to be copied to another collection.
		/// </summary>
		Copy=256,
		/// <summary>
		/// Allows the collection to be copied to, from another collection.
		/// </summary>
		Paste=512,
		/// <summary>
		/// Allows the collection to be converted into another enumerable data-type.
		/// </summary>
		Conversion=1024,
		/// <summary>
		/// Full access to adjusting the collection.
		/// </summary>
		Unlocked=2048,
	}
}