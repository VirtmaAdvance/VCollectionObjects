namespace VCollectionObjects.Extensions
{
	internal static class ArrayPushExt
	{

		public static T[] Push<T>(this T[] array, T value)
		{
			if(array is null)
				array=[];
			Array.Resize(ref array, array.Length+1);
			array[^1]=value;
			return array;
		}

	}
}