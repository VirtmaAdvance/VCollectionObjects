using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace VCollectionObjects
{
	/// <summary>
	/// Provides a more advanced means to manages a collection of items in a one-dimensional array.
	/// </summary>
	/// <typeparam name="T">The <see cref="Type"/> to use for the values stored within this collection.</typeparam>
	public class VEnumerable<T>:IEnumerable, IEnumerable<T?>
	{
		/// <summary>
		/// The collection of items.
		/// </summary>
		protected T?[] Items=[];
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
		/// The number of items within this collection.
		/// </summary>
		public int Length => Items.Length;
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
		/// <summary>
		/// Gets or sets the value at a given <paramref name="index"/> in the collection.
		/// </summary>
		/// <remarks>If the index is out of range when setting a value, the collection size will be adjusted to accomodate the index position.</remarks>
		/// <param name="index">The index position to get or set the the value.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		protected T? this[int index]
		{
			get => IsIndexValid(index) ? Items[index] : throw new ArgumentOutOfRangeException(nameof(index));
			set
			{
				if(IsIndexValid(index))
					Items[index]=value;
				else
					Insert(index, value);
			}
		}


		/// <summary>
		/// Creates a new instance of the <see cref="VEnumerable{T}"/> collection.
		/// </summary>
		public VEnumerable() { }
		/// <inheritdoc cref="VEnumerable{T}"/>
		public VEnumerable(IEnumerable<T> value) => Items=value.ToArray();
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
		public void Add(T item)
		{
			if(PrvCheckIfLocked())
			{
				Adding?.Invoke(this, item);
				Resize(Length+1);
				Items[^1]=item;
				Added?.Invoke(this, item);
			}
		}
		/// <inheritdoc cref="Add(T)"/>
		public void Add(params T[] items)
		{
			if(PrvCheckIfLocked())
			{
				foreach(var sel in items)
					Add(sel);
			}
		}
		/// <summary>
		/// Determines if the collection contains the <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to look for.</param>
		/// <returns>a <see cref="bool">boolean</see> value representing success or failure.</returns>
		public bool Contains(T? item) => Items.Contains(item);
		/// <summary>
		/// Removes an item from the collection.
		/// </summary>
		/// <param name="item"></param>
		public void Remove(T? item)
		{
			if(PrvCheckIfLocked())
			{
				Removing?.Invoke(this, item);
				if(Contains(item))
				{
					if(UseShiftingWhenRemoving)
						ShiftLeft(IndexOf(item), 1);
					else
						IterateAcceptValid(ref Items, q => IsValueEqual(item, q));
				}
			}
		}
		/// <summary>
		/// Inserts an item into the collection at the given <paramref name="index"/> position.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected void Insert(int index, T? item)
		{
			if(PrvCheckIfLocked())
			{
				if(IsIndexValid(index))
				{
					ShiftRight(index, 1);
					Items[index]=item;
					Added?.Invoke(this, index);
				}
			}
		}
		/// <summary>
		/// Prepends an item to the collection.
		/// </summary>
		/// <param name="items"></param>
		protected void Prepend(params T?[] items)
		{
			if(PrvCheckIfLocked())
			{
				int len=items.Length;
				Move(0, len);
				for(int i = 0;i<len;i++)
					Items[i]=items[i];
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
			{
				int len=Length;
				for(int i = len;i>=startingIndex;i--)
					Move(i, i+shiftLength);
			}
		}
		/// <summary>
		/// Shifts the items starting from the <paramref name="startingIndex"/> to the left of the array <paramref name="shiftLength"/> many times.
		/// </summary>
		/// <param name="startingIndex">The index to start shifting the elements at.</param>
		/// <param name="shiftLength">The number of places to move the elements by.</param>
		protected void ShiftLeft(int startingIndex, int shiftLength)
		{
			if(PrvCheckIfLocked())
			{
				for(int i = startingIndex;i<startingIndex+shiftLength;i++)
					Move(i, i-shiftLength);
			}
		}
		/// <summary>
		/// Copys the element at the <paramref name="sourceIndex"/> to the <paramref name="destinationIndex"/>.
		/// </summary>
		/// <param name="sourceIndex">The index of the element to move.</param>
		/// <param name="destinationIndex">The index position to move the element at the <paramref name="sourceIndex"/> to.</param>
		protected void Move(int sourceIndex, int destinationIndex)
		{
			if(PrvCheckIfLocked())
			{
				if(IsIndexValid(sourceIndex))
				{
					T? tmp=Items[sourceIndex];
					if(destinationIndex>=Length)
						Resize(ref Items, destinationIndex+1);
					else if(destinationIndex<0)
					{
						ShiftRight(0, Math.Abs(destinationIndex));
						destinationIndex=0;
					}
					Items[destinationIndex]=tmp;
					//Moved?.Invoke(this, new object[] { sourceIndex, destinationIndex });
				}
			}
		}
		/// <summary>
		/// Iterates through the <paramref name="array"/> and only includes the items that pass the <paramref name="predicate"/>.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="predicate"></param>
		protected static void IterateAcceptValid(ref T?[] array, Func<T?, bool> predicate)
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
		/// <summary>
		/// Gets the index position of the first occurence of a matching value.
		/// </summary>
		/// <param name="item">The value to look for.</param>
		/// <returns>an <see cref="int"/> representing the index position.</returns>
		protected int IndexOf(T? item)
		{
			for(int i=0;i<Length;i++)
				if(IsValueEqual(item, Items[i]))
					return i;
			return -1;
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
				Resize(ref Items, length);
				Resized?.Invoke(this, length);
			}
		}
		/// <summary>
		/// Resizes the collection.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="length"></param>
		protected static void Resize(ref T?[] array, int length) => Array.Resize(ref array, length);
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
		public IEnumerator GetEnumerator() => Items.GetEnumerator();
		/// <inheritdoc cref="GetEnumerator()"/>
		IEnumerator<T?> IEnumerable<T?>.GetEnumerator() => (IEnumerator<T?>)GetEnumerator();
	}
}
