namespace VCollectionObjects
{
	/// <inheritdoc cref="VEnumerable{T}"/>
	public class VArray<T> : VEnumerable<T>
	{

		public new T this[int index]
		{
			get => this[index];
			set => this[index] = value;
		}

		/// <inheritdoc cref="VEnumerable{T}.Add(T)"/>
		public new void Add(T value) => base.Add(value);
		/// <inheritdoc cref="VEnumerable{T}.Add(T[])"/>
		public new void Add(params T[] values) => base.Add(values);
		/// <inheritdoc cref="VEnumerable{T}.Remove(T)"/>
		public new void Remove(T value) => base.Remove(value);
		/// <inheritdoc cref="VEnumerable{T}.Remove(T)"/>
		public void Remove(params T[] values)
		{
			foreach(var sel in values)
				base.Remove(sel);
		}
		/// <inheritdoc cref="VEnumerable{T}.Clear()"/>
		public new void Clear() => base.Clear();
		/// <inheritdoc cref="VEnumerable{T}.IndexOf(T)"/>
		public new int IndexOf(T item) => base.IndexOf(item);
		/// <inheritdoc cref="VEnumerable{T}.Contains(T)"/>
		public new bool Contains(T item) => base.Contains(item);

	}
}