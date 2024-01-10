namespace VCollectionObjects
{
	internal static class InternalTypeInfoExt
	{

		public static bool Is(this Type type, params Type[] types) => types.Any(type.IsAssignableTo);

		public static bool IsNumber(this Type type) => type.Is(typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(nint), typeof(long), typeof(ulong), typeof(double), typeof(float), typeof(decimal));

		public static bool IsNumber(this object? value) => value is null ? false : IsNumber(value is Type ? (Type)value : value.GetType());

	}
}