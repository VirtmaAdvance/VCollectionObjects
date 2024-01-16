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
		/// <param name="key">The <typeparamref name="TKey"/> to get or set the value.</param>
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
		/// <summary>
		/// Sets the <paramref name="value"/> at the given <paramref name="key"/>.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		protected void SetValueAtKey(TKey key, TValue value)
		{
			int index=IndexOf(key);
			if(index>-1)
				Items[index]=new KeyValuePair<TKey, TValue>(key, value);
			else
				Add(key, value);
		}
		/// <summary>
		/// Gets the index position of the first occurence of a matching value.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		protected int IndexOf(TKey key)
		{
			for(int i=0;i<Length;i++)
				if(IsValueEqual(Items[i].Key, key))
					return i;
			return -1;
		}
		/// <inheritdoc cref="IndexOf(TKey)"/>
		/// <summary>
		/// Gets the index position of the first occurence of a matching value using multi-threading methods.
		/// </summary>
		protected int ParallelIndexOf(TKey key)
		{
			int res=-1;
			object lockObject=new ();
			Parallel.For(0, Length, (i, pls) =>
			{
				if(IsValueEqual(Items[i], key))
					lock(lockObject)
					{
						res=i;
						pls.Break();
					}
			});
			return res;
		}
		/// <summary>
		/// Gets the pair that has a matching <paramref name="key"/>.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException"></exception>
		protected KeyValuePair<TKey, TValue> GetPairFromKey(TKey key)
		{
			var index=ParallelIndexOf(key);
			return IsIndexValid(index) ? Items[index] : throw new KeyNotFoundException("The given key was not found within the collection.");
			//foreach(var sel in this)
			//	if(IsValueEqual(sel, key))
			//		return sel;
			//throw new KeyNotFoundException("The given key was not found within the collection.");
		}
		/// <summary>
		/// Gets all pairs that match the <paramref name="selector"/>.
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		protected KeyValuePair<TKey, TValue>[] GetPairs(Func<KeyValuePair<TKey, TValue>, bool> selector)
		{
			KeyValuePair<TKey, TValue>[] res=[];
			foreach(var sel in this)
				if(selector(sel))
					AppendToArray(ref res, sel);
			return res;
		}
		/// <inheritdoc cref="GetPairs(Func{KeyValuePair{TKey, TValue}, bool})"/>
		/// <summary>
		/// Gets all pairs that match the <paramref name="selector"/> using multi-threading methods.
		/// </summary>
		protected KeyValuePair<TKey, TValue>[] ParallelGetPairs(Func<KeyValuePair<TKey, TValue>, bool> selector)
		{
			KeyValuePair<TKey, TValue>[] res=[];
			object lockObject=new ();
			Parallel.ForEach(Items, (item, pls)=>
			{
				if(selector(item))
					lock(lockObject)
						AppendToArray(ref res, item);
			});
			return res;
		}
		/// <summary>
		/// Iterates through the given <paramref name="array"/> and replaces any duplicate keys stored within the collection.
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		protected static KeyValuePair<TKey, TValue>[] CompressEnforceUniqueKey(KeyValuePair<TKey, TValue>[] array)
		{
			KeyValuePair<TKey, TValue>[] res=[];
			foreach(var sel in array)
				if(!StaticContainsKey(res, sel.Key))
					AppendToArray(ref res, sel);
			return res;
		}
		/// <inheritdoc cref="CompressEnforceUniqueKey(KeyValuePair{TKey, TValue}[])"/>
		/// <summary>
		/// Iterates through the given <paramref name="array"/> and replaces any duplicate keys stored within the collection using multi-threading methods.
		/// </summary>
		protected static KeyValuePair<TKey, TValue>[] ParallelCompressEnforceUniqueKey(KeyValuePair<TKey, TValue>[] array)
		{
			KeyValuePair<TKey, TValue>[] res=[];
			object lockObject=new ();
			Parallel.ForEach(array, (sel, pls) =>
			{
				if(!StaticContainsKey(res, sel.Key))
					lock(lockObject)
						AppendToArray(ref res, sel);
			});
			return res;
		}
		/// <summary>
		/// Appends a value to an array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="value"></param>
		protected static void AppendToArray(ref KeyValuePair<TKey, TValue>[] array, KeyValuePair<TKey, TValue> value)
		{
			Array.Resize(ref array, array.Length+1);
			array[^1]=value;
		}
		/// <summary>
		/// Determines if the collection contains the <paramref name="key"/>.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		protected static bool StaticContainsKey(KeyValuePair<TKey, TValue>[] array, TKey key) => array.Any(q=>IsValueEqual(q.Key, key));

	}
}