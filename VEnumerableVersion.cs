namespace VCollectionObjects
{
	internal struct VEnumerableVersion
	{

		public readonly string VersionName;

		public readonly string MajorVersion;

		public readonly string MinorVersion;

		public string Version => MajorVersion + "." + MinorVersion;



		public VEnumerableVersion(string name, string majorVersion, string minorVersion)
		{
			VersionName = name;
			MajorVersion = majorVersion;
			MinorVersion = minorVersion;
		}

	}
}