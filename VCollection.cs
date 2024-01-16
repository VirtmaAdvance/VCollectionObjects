namespace VCollectionObjects
{
	/// <summary>
	/// Manages a collection of items.
	/// </summary>
	/// <typeparam name="TKey">The <see cref="Type"/> object for the key.</typeparam>
	/// <typeparam name="TValue">The <see cref="Type"/> object for the value.</typeparam>
	public class VCollection<TKey, TValue> : VEnumerable<KeyValuePair<TKey, TValue>>
	{
		private bool _enforceUniqueKeys=false;
		/// <summary>
		/// A <see cref="bool">boolean</see> value representing if all keys in the collection should be unique or allowed to have multiple copies of the same key.
		/// </summary>
		protected bool EnforceUniqueKeys
		{
			get => _enforceUniqueKeys;
			set
			{
				_enforceUniqueKeys = value;
				if(value)
					Items=CompressEnforceUniqueKey(Items);
			}
		}
		/// <summary>
		/// Gets or sets the value at the given <paramref name="key"/>.
		/// </summary>
		/// <param name="key">The <see cref="TKey"/> to get or set the value.</param>
		/// <returns>the <typeparamref name="TValue"/>.</returns>
		public TValue this[TKey key]
		{
			get => GetPairFromKey(key).Value;
			set => SetValueAtKey(key, value);
		}


		/// <inheritdoc cref="VEnumerable{T}.Add(T)"/>
		public void Add(TKey key, TValue value) => Add(new KeyValuePair<TKey, TValue>(key, value));
		/// <inheritdoc cref="VEnumerable{T}.Add(T)"/>
		public new void Add(KeyValuePair<TKey, TValue> item)
		{
			if(!EnforceUniqueKeys || (EnforceUniqueKeys && !ContainsKey(item.Key)))
				base.Add(item);
		}
		/// <inheritdoc cref="VEnumerable{T}.Add(T)"/>
		public new void Add(params KeyValuePair<TKey, TValue>[] items)
		{
			foreach(var sel in items)
				Add(sel);
		}
		/// <inheritdoc cref="VEnumerable{T}.Contains(T)"/>
		public bool ContainsKey(TKey key) => Any(q=>IsValueEqual(q.Key, key));
		/// <inheritdoc cref="VEnumerable{T}.Contains(T)"/>
		public bool ContainsValue(TValue value) => Any(q=>IsValueEqual(q.Value, value));
		/// <inheritdoc cref="VEnumerable{T}.Remove(T)"/>
		public void Remove(TKey key)
		{
			if(ContainsKey(key))
				RemoveAt(IndexOf(key));
		}
		/// <inheritdoc cref="Remove(TKey)"/>
		public void Remove(params TKey[] keys)
		{
			foreach(var key in keys)
				Remove(key);
		}
		/// <inheritdoc cref="VEnumerable{T}.Clear()"/>
		public new void Clear() => base.Clear();

		protected void SetValueAtKey(TKey key, TValue value)
		{
			int index=IndexOf(key);
			if(index>-1)
				Items[index]=new KeyValuePair<TKey, TValue>(key, value);
			else
				Add(key, value);
		}

		protected int IndexOf(TKey key)
		{
			for(int i=0;i<Length;i++)
				if(IsValueEqual(Items[i].Key, key))
					return i;
			return -1;
		}

		protected KeyValuePair<TKey, TValue> GetPairFromKey(TKey key)
		{
			foreach(var sel in this)
				if(IsValueEqual(sel, key))
					return sel;
			throw new KeyNotFoundException("The given key was not found within the collection.");
		}

		protected KeyValuePair<TKey, TValue>[] GetPairs(Func<KeyValuePair<TKey, TValue>, bool> selector)
		{
			KeyValuePair<TKey, TValue>[] res=[];
			foreach(var sel in this)
				if(selector(sel))
					AppendToArray(ref res, sel);
			return res;
		}

		protected static KeyValuePair<TKey, TValue>[] CompressEnforceUniqueKey(KeyValuePair<TKey, TValue>[] array)
		{
			KeyValuePair<TKey, TValue>[] res=[];
			foreach(var sel in array)
				if(!StaticContainsKey(res, sel.Key))
					AppendToArray(ref res, sel);
			return res;
		}

		protected static void AppendToArray(ref KeyValuePair<TKey, TValue>[] array, KeyValuePair<TKey, TValue> value)
		{
			Array.Resize(ref array, array.Length+1);
			array[^1]=value;
		}

		protected static bool StaticContainsKey(KeyValuePair<TKey, TValue>[] array, TKey key) => array.Any(q=>IsValueEqual(q.Key, key));

	}
}