namespace VCollectionObjects
{
	/// <summary>
	/// Defines a collection event handler.
	/// </summary>
	/// <param name="sender">The object referencing the source that invoked this event.</param>
	/// <param name="data">The data to send to the invoking method.</param>
	public delegate void GenericCollectionEvent(object? sender, params object?[]? data);
}
