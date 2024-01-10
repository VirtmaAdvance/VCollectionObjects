namespace VCollectionObjects
{
	/// <summary>
	/// Provides access flags for collections.
	/// </summary>
	[Flags]
	public enum CollectionAccessStateFlags : int
	{
		/// <summary>
		/// No adjustments can be made.
		/// </summary>
		Locked=0,
		/// <summary>
		/// Allowed to read the collection and it's contents.
		/// </summary>
		Read=1,
		/// <summary>
		/// Allowed to add new items to the collection.
		/// </summary>
		Add=2,
		/// <summary>
		/// Allowed to remove items from the collection.
		/// </summary>
		Remove=4,
		/// <summary>
		/// Allowed to move items within the collection (Includes replacing other items).
		/// </summary>
		Move=8,
		/// <summary>
		/// Same as <see cref="Move"/>, but will adjust the size of the collection afterwards (Involving the removal of items within the collection).
		/// </summary>
		Shift=16,
		/// <summary>
		/// Full access to adjusting the collection.
		/// </summary>
		Unlocked=32,
	}
}