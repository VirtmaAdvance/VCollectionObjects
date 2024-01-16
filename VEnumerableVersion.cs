using System.Runtime.InteropServices;

namespace VCollectionObjects
{
	/// <summary>
	/// An internal struct to store version information.
	/// </summary>
	/// <remarks>
	/// Creates a new instance of the <see cref="VEnumerableVersion"/> struct.
	/// </remarks>
	/// <param name="name"></param>
	/// <param name="majorVersion"></param>
	/// <param name="minorVersion"></param>
	[Guid("2D32D5F7-85BF-4FE7-951A-D0081E4A4CEE")]
	internal struct VEnumerableVersion(string name, string majorVersion, string minorVersion)
	{

		public readonly string VersionName = name;

		public readonly string MajorVersion = majorVersion;

		public readonly string MinorVersion = minorVersion;

		public string Version => MajorVersion + "." + MinorVersion;
	}
}