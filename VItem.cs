namespace VCollectionObjects
{
	/// <summary>
	/// A structured object used to store a pair of values.
	/// </summary>
	/// <remarks>
	/// Creates a new instance of the <see cref="VItem"/> struct object.
	/// </remarks>
	/// <param name="key">The key to associate with the <paramref name="value"/>.</param>
	/// <param name="value">The value to associate with the <paramref name="key"/>.</param>
	public struct VItem(object? key, object? value)
	{
		/// <summary>
		/// The associated key.
		/// </summary>
		public object? Key { get; set; } = key;
		/// <summary>
		/// The associated value.
		/// </summary>
		public object? Value { get; set; } = value;

		/// <inheritdoc cref="VItem(object? object?)"/>
		public VItem(object? value) : this(null, value) {}

	}
}