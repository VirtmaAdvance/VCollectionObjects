using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace VCollectionObjects
{
	/// <summary>
	/// Provides a more advanced means to manages a collection of items in a one-dimensional array.
	/// </summary>
	/// <typeparam name="T">The <see cref="Type"/> to use for the values stored within this collection.</typeparam>
	[Guid("DFE94998-CD37-4930-8D12-1F33945D33EC")]
	public class VEnumerable<T> : IVEnumerable, IEnumerable<T>
	{

		internal static VEnumerableVersion VersionInfo = new ("Archon","0.0","0.0");
		private T[] _items=[];
		/// <summary>
		/// The collection of items.
		/// </summary>
		public T[] Items
		{
			get => _items;
			set => _items = value??[];
		}
		/// <summary>
		/// Invoked right before adding an item to the collection.
		/// </summary>
		protected event GenericCollectionEvent? Adding;
		/// <summary>
		/// Invoked after an item has been added to the collection.
		/// </summary>
		protected event GenericCollectionEvent? Added;
		/// <summary>
		/// Invoked right before removing an item from the collection.
		/// </summary>
		protected event GenericCollectionEvent? Removing;
		/// <summary>
		/// Invoked after an item was removed from the collection.
		/// </summary>
		protected event GenericCollectionEvent? Removed;
		/// <summary>
		/// Invoked when an item within the collection was updated.
		/// </summary>
		protected event GenericCollectionEvent? Updated;
		/// <summary>
		/// Invoked right before clearing the collection.
		/// </summary>
		protected event GenericCollectionEvent? Clearing;
		/// <summary>
		/// Invoked after the collection has been cleared.
		/// </summary>
		protected event GenericCollectionEvent? Cleared;
		/// <summary>
		/// Invoked when the collection was resized.
		/// </summary>
		protected event GenericCollectionEvent? Resized;
		/// <summary>
		/// Invoked right before an item is moved to another index within the collection.
		/// </summary>
		/// <remarks>Provides the collection object as the first argument, and an array consisting of the index of the object being moved, and the index the object was moved to.</remarks>
		protected event GenericCollectionEvent? Moving;
		/// <summary>
		/// Invoked when an item within the collection was moved/transferred to another index within the collection.
		/// </summary>
		/// <remarks>Provides the collection object as the first argument, and an array consisting of the index of the object being moved, and the index the object was moved to.</remarks>
		protected event GenericCollectionEvent? Moved;
		/// <summary>
		/// The number of items within this collection.
		/// </summary>
		public int Length => _items.Length;
		/// <summary>
		/// Indicates that all items will be shifted to the left when an item is removed to account for the removal of the item.
		/// </summary>
		protected bool UseShiftingWhenRemoving { get; set; } = true;
		/// <summary>
		/// When set to <see cref="bool">true</see>, prevents any and all changes to be made to the collection from outside of this class.
		/// </summary>
		protected bool Locked { get; set; } = false;
		/// <summary>
		/// If the collection is locked and this value is set to <see cref="bool">true</see>, then an exception will be thrown.
		/// </summary>
		protected bool ThrowExceptionIfLocked { get; set; } = false;
		/// <summary>
		/// Specifies the access state for this collection.
		/// </summary>
		protected CollectionAccessStateFlags AccessState { get; set; } = CollectionAccessStateFlags.Unlocked;

		int ICollection.Count => throw new NotImplementedException();
		/// <inheritdoc cref="ICollection.IsSynchronized"/>
		public bool IsSynchronized => throw new NotImplementedException();
		/// <inheritdoc cref="ICollection.SyncRoot"/>
		public object SyncRoot => throw new NotImplementedException();

		/// <summary>
		/// Gets or sets the value at a given <paramref name="index"/> in the collection.
		/// </summary>
		/// <remarks>If the index is out of range when setting a value, the collection size will be adjusted to accomodate the index position.</remarks>
		/// <param name="index">The index position to get or set the the value.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		protected T this[int index]
		{
			get => IsIndexValid(index) ? _items[index] : throw new ArgumentOutOfRangeException(nameof(index));
			set
			{
				if(IsIndexValid(index))
					_items[index]=value;
				else
					Insert(index, value);
			}
		}


		/// <summary>
		/// Creates a new instance of the <see cref="VEnumerable{T}"/> collection.
		/// </summary>
		public VEnumerable() { }
		/// <inheritdoc cref="VEnumerable{T}"/>
		public VEnumerable(IEnumerable<T> value) => _items=value.ToArray();
		/// <inheritdoc cref="VEnumerable{T}"/>
		public static implicit operator VEnumerable<T>(List<T> value) => new(value.ToArray());
		/// <inheritdoc cref="VEnumerable{T}"/>
		public static implicit operator VEnumerable<T>(Array value) => new(value.Cast<T>());

		private bool PrvCheckIfLocked()
		{
			bool res=false;
			if(PrvCheckAccessState(AccessState))
				res=true;
			if(Locked)
				res=false;
			//if(!res && ThrowExceptionIfLocked)
			//	throw new Exception("Attempted to modify the collection while the collection is locked.");
			return !res && ThrowExceptionIfLocked ? throw new Exception("Attempted to modify the collection while the collection is locked.") : res;
		}

		private static bool PrvCheckAccessState(CollectionAccessStateFlags accessState)
		{
			bool res=false;
			var m=GetCallerMethod(3);
			if(m is not null)
			{
				int score=0;
				string name=m.Name;
				if(!(accessState.HasFlag(CollectionAccessStateFlags.Read) && PrvContains(name, "IndexOf", "Contains", "GetValue")))
					score+=1;
				if(!(accessState.HasFlag(CollectionAccessStateFlags.Remove) && PrvContains(name, "Remove")))
					score+=2;
				if(!(accessState.HasFlag(CollectionAccessStateFlags.Add) && PrvContains(name, "Add", "Insert", "Update", "Prepend")))
					score+=4;
				if(!(accessState.HasFlag(CollectionAccessStateFlags.Move) && PrvContains(name, "Move")))
					score+=8;
				if(!(accessState.HasFlag(CollectionAccessStateFlags.Shift) && PrvContains(name, "Shift")))
					score+=16;
				if(!accessState.HasFlag(CollectionAccessStateFlags.Unlocked))
					score+=32;
				res=score<=((int)accessState);
			}
			return res;
		}

		private static bool PrvContains(string source, params string[] values) => values.Any(source.Contains);

		private static MethodBase? GetCallerMethod(int index=2)
		{
			StackTrace st=new ();
			StackFrame[] frames=st.GetFrames();
			if(frames is not null && frames.Length>index)
				return frames[^(index+1)].GetMethod();
			return null;
		}
		/// <summary>
		/// Adds an item to the collection.
		/// </summary>
		/// <param name="item">The item to add to the collection.</param>
		protected void Add(T item)
		{
			if(PrvCheckIfLocked())
			{
				Adding?.Invoke(this, item);
				Resize(Length+1);
				_items[^1]=item;
				Added?.Invoke(this, item);
			}
		}
		/// <inheritdoc cref="Add(T)"/>
		protected void Add(params T[] items)
		{
			if(PrvCheckIfLocked())
				foreach(var sel in items)
					Add(sel);
		}
		/// <summary>
		/// Determines if the collection contains the <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to look for.</param>
		/// <returns>a <see cref="bool">boolean</see> value representing success or failure.</returns>
		protected bool Contains(T? item) => PrvContains(item);

		private bool PrvContains(T? item) => _items.Contains(item);
		/// <summary>
		/// Gets the element at the given <paramref name="index"/> position in the collection.
		/// </summary>
		/// <param name="index">An <see cref="int"/> value used to reference the index position of the element to get.</param>
		/// <returns>the <typeparamref name="T"/> object found at the specified <paramref name="index"/> position.</returns>
		public T GetElementAt(int index) => _items[index];
		/// <summary>
		/// Attempts to get the element at the given <paramref name="index"/> position.
		/// </summary>
		/// <param name="index">An <see cref="int"/> value used to reference the index position of the element to get.</param>
		/// <returns>the <typeparamref name="T"/> object found at the specified <paramref name="index"/> position.</returns>
		public T? TryGetElementAt(int index) => IsIndexValid(index) ? _items[index] : default;
		/// <summary>
		/// Removes an item from the collection.
		/// </summary>
		/// <param name="item"></param>
		protected void Remove(T item)
		{
			if(PrvCheckIfLocked() && PrvContains(item))
			{
				Removing?.Invoke(this, item);
				PrvRemoveOp(item);
			}
		}
		/// <summary>
		/// Removes the item at the given <paramref name="index"/>.
		/// </summary>
		/// <param name="index"></param>
		protected void RemoveAt(int index)
		{
			if(PrvCheckIfLocked() && IsIndexValid(index))
			{
				T item=GetElementAt(index);
				Removing?.Invoke(this, item);
				PrvRemoveOp(item);
			}
		}

		private void PrvRemoveOp(T item)
		{
			if(UseShiftingWhenRemoving)
				ShiftLeft(IndexOf(item), 1);
			else
				IterateAcceptValid(ref _items, q => IsValueEqual(item, q));
		}
		/// <inheritdoc cref="Array.Clear(Array)"/>
		protected void Clear()
		{
			if(PrvCheckIfLocked())
			{
				Clearing?.Invoke(this, null);
				Array.Clear(_items);
				Cleared?.Invoke(this, null);
			}
		}
		/// <summary>
		/// Inserts an item into the collection at the given <paramref name="index"/> position.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected void Insert(int index, T item)
		{
			if(PrvCheckIfLocked() && IsIndexValid(index))
				PrvInsertOp(index, item);
		}

		private void PrvInsertOp(int index, T item)
		{
			ShiftRight(index, 1);
			_items[index]=item;
			Added?.Invoke(this, index);
		}
		/// <summary>
		/// Prepends an item to the collection.
		/// </summary>
		/// <param name="items"></param>
		protected void Prepend(params T[] items)
		{
			if(PrvCheckIfLocked())
			{
				int len=items.Length;
				Move(0, len);
				for(int i = 0;i<len;i++)
					_items[i]=items[i];
				Added?.Invoke(this, null);
			}
		}
		/// <inheritdoc cref="ShiftLeft(int, int)"/>
		/// <summary>
		/// Shifts the items starting from the <paramref name="startingIndex"/> to the right of the array <paramref name="shiftLength"/> many times.
		/// </summary>
		protected void ShiftRight(int startingIndex, int shiftLength)
		{
			if(PrvCheckIfLocked())
				for(int i = Length;i>=startingIndex;i--)
					Move(i, i+shiftLength);
		}
		/// <summary>
		/// Shifts the items starting from the <paramref name="startingIndex"/> to the left of the array <paramref name="shiftLength"/> many times.
		/// </summary>
		/// <param name="startingIndex">The index to start shifting the elements at.</param>
		/// <param name="shiftLength">The number of places to move the elements by.</param>
		protected void ShiftLeft(int startingIndex, int shiftLength)
		{
			if(PrvCheckIfLocked())
				for(int i = startingIndex;i<startingIndex+shiftLength;i++)
					Move(i, i-shiftLength);
		}
		/// <summary>
		/// Copys the element at the <paramref name="sourceIndex"/> to the <paramref name="destinationIndex"/>.
		/// </summary>
		/// <param name="sourceIndex">The index of the element to move.</param>
		/// <param name="destinationIndex">The index position to move the element at the <paramref name="sourceIndex"/> to.</param>
		protected void Move(int sourceIndex, int destinationIndex)
		{
			if(PrvCheckIfLocked() && IsIndexValid(sourceIndex))
				PrvMoveOp(sourceIndex, destinationIndex);
		}

		private void PrvMoveOp(int sourceIndex, int destinationIndex)
		{
			T? tmp=_items[sourceIndex];
			destinationIndex=PrvMoveOpCond(destinationIndex);
			_items[destinationIndex]=tmp;
		}

		private int PrvMoveOpCond(int destinationIndex) => destinationIndex>=Length ? PrvMoveOpResize(destinationIndex) : destinationIndex<0 ? PrvMoveShiftRightOpWhenLessThanZero(destinationIndex) : destinationIndex;

		private int PrvMoveOpResize(int destinationIndex)
		{
			Resize(ref _items, destinationIndex+1);
			return destinationIndex;
		}

		private int PrvMoveShiftRightOpWhenLessThanZero(int destinationIndex)
		{
			ShiftRight(0, Math.Abs(destinationIndex));
			return 0;
		}
		/// <summary>
		/// Iterates through the <paramref name="array"/> and only includes the items that pass the <paramref name="predicate"/>.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="predicate"></param>
		protected static void IterateAcceptValid(ref T[] array, Func<T, bool> predicate)
		{
			var tmp=array;
			Array.Clear(array);
			foreach(var sel in tmp)
				if(predicate(sel))
				{
					Resize(ref array, array.Length+1);
					array[^1]=sel;
				}
		}
		/// <inheritdoc cref="Enumerable.Count{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
		public int Count(Func<T, bool> predicate) => _items.Count(predicate);
		/// <inheritdoc cref="Enumerable.Count{TSource}(IEnumerable{TSource})"/>
		public int Count() => _items is not null ? _items.Count() : 0;
		/// <summary>
		/// Gets the index position of the first occurence of a matching value.
		/// </summary>
		/// <param name="item">The value to look for.</param>
		/// <returns>an <see cref="int"/> representing the index position.</returns>
		protected int IndexOf(T item)
		{
			for(int i=0;i<Length;i++)
				if(IsValueEqual(item, _items[i]))
					return i;
			return -1;
		}
		/// <inheritdoc cref="IndexOf(T)"/>
		/// <summary>
		/// Gets the index position of the first occurence of a matching value using multi-threading methods.
		/// </summary>
		protected int ParallelIndexOf(T item)
		{
			int res=-1;
			object lockObject=new ();
			Parallel.For(0, Length, (i, pls) =>
			{
				if(IsValueEqual(item, _items[i]))
					lock(lockObject)
					{
						res=i;
						pls.Break();
					}
			});
			return res;
		}
		/// <summary>
		/// Determines if two values are equal to each other.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		protected static bool IsValueEqual(object? a, object? b) => (a is null && b is null) || (a is not null && b is not null && a.Equals(b));
		/// <summary>
		/// Resizes the collection.
		/// </summary>
		/// <param name="length"></param>
		private void Resize(int length)
		{
			if(PrvCheckIfLocked())
			{
				Resize(ref _items, length);
				Resized?.Invoke(this, length);
			}
		}
		/// <summary>
		/// Resizes the collection.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="length"></param>
		protected static void Resize(ref T[] array, int length) => Array.Resize(ref array, length);
		/// <summary>
		/// Determines if the given <paramref name="index"/> is a valid index within the collection range.
		/// </summary>
		/// <param name="index">The index to test.</param>
		/// <returns>a <see cref="bool">boolean</see> value representing success or failure.</returns>
		protected bool IsIndexValid(int index) => index>-1 && index<Length;
		/// <summary>
		/// Gets the value that the <paramref name="value"/> is closest to.
		/// </summary>
		/// <param name="value">The value to test for.</param>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>either the <paramref name="min"/> or <paramref name="max"/> value given that the <paramref name="value"/> is closest to.</returns>
		protected static int GetClosest(int value, int min, int max) => Math.Max(min, value) - Math.Min(min, value)<Math.Max(max, value) - Math.Min(max, value) ? min : max;
		/// <summary>
		/// Gets the enumerator for this object.
		/// </summary>
		/// <returns>the <see cref="IEnumerator{T}"/> for the collection.</returns>
		public IEnumerator<T> GetEnumerator() => (IEnumerator<T>)_items.GetEnumerator();
		/// <inheritdoc cref="GetEnumerator()"/>
		IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>)GetEnumerator();
		/// <summary>
		/// Gets the <see cref="IEnumerator{T}"/> representation of this object.
		/// </summary>
		/// <returns>the <see cref="IEnumerator{T}"/> representation of this object.</returns>
		public IEnumerator<T> ToEnumerator() => GetEnumerator();
		/// <summary>
		/// Gets the array representation of this object.
		/// </summary>
		/// <returns>the <typeparamref name="T"/>[] representation of this object.</returns>
		public T[] ToArray() => _items;
		/// <summary>
		/// Gets the <see cref="List{T}"/> representation of this object.
		/// </summary>
		/// <returns>the <see cref="List{T}"/> representation of this object.</returns>
		public List<T> ToList() => _items.ToList();
		/// <inheritdoc cref="object.ToString()"/>
		public new string? ToString() => _items.ToString();
		/// <summary>
		/// Gets a JSON string representation of this collection.
		/// </summary>
		/// <returns>a <see cref="string"/> formatted as a JSON.</returns>
		public string ToJsonString() => GetStringValue(this);
		/// <summary>
		/// Gets the <see cref="string"/> representation of the given <paramref name="value"/>.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected static string GetStringValue(object? value)
		{
			if(value is null)
				return "null";
			if(System.Runtime.InteropServices.Marshal.IsComObject(value))
				return "COMObject";
			if(value is string stringValue)
				return "\""+stringValue+"\"";
			if(value is char charValue)
				return "'"+charValue+"'";
			if(value is bool boolValue)
				return boolValue ? "true" : "false";
			if(value is Type typeValue)
				return typeValue.Name;
			if(IsKeyValurPair(value))
				return GetKVPString(value);
			if(value is IEnumerable iEnumerableValue)
				return GetStringValueOfEnumerable(iEnumerableValue);
			return value.ToString()!;
		}

		private static string GetStringValueOfEnumerable(IEnumerable source)
		{
			string tmp="";
			object lockObject=new();
			Parallel.ForEach(source.Cast<object?>(), (item, pls)=>
			{
				string value=GetStringValue(item);
				lock(lockObject)
					tmp+=(tmp.Length>0 ? "," : "") + value;
			});
			//foreach(var sel in source)
			//	tmp+=(tmp.Length>0 ? "," : "") + GetStringValue(sel);
			return source is IDictionary ? "{"+tmp+"}" : "["+tmp+"]";
		}

		private static string GetKVPString(object obj)
		{
			var pInfo=obj.GetType().GetProperties();
			return GetStringValue(pInfo[0].GetValue(obj)) + ":" + GetStringValue(pInfo[1].GetValue(obj));
		}

		private static bool IsKeyValurPair(object obj) => (obj is not null) && obj.GetType().Name.Contains("keyvaluepair", StringComparison.CurrentCultureIgnoreCase);
		/// <summary>
		/// Disposes this object.
		/// </summary>
		public void Dispose() => Array.Clear(_items);
		/// <summary>
		/// Gets the object data.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		/// <exception cref="NotImplementedException"></exception>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
			//ArgumentNullException.ThrowIfNull(info);
			//info.AddValue(VersionInfo.VersionName, VersionInfo.Version);
			//// Continue...
		}
		/// <summary>
		/// Performs an operation on deserialization.
		/// </summary>
		/// <param name="sender"></param>
		/// <exception cref="NotImplementedException"></exception>
		public void OnDeserialization(object? sender)
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// Compares this object to another.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <exception cref="InvalidDataException"></exception>
		public int CompareTo(object? obj)
		{
			if(obj is IVEnumerable value)
				return value.Length<Length ? -1 : value.Length>Length ? 1 : 0;
			else
				throw new InvalidDataException();

		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		/// <summary>
		/// Copies this object to another.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		/// <exception cref="NotImplementedException"></exception>
		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

	}
}