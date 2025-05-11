namespace Massive
{
	public readonly struct TypeIdInfo
	{
		public readonly int Index;
		public readonly string FullName;

		public TypeIdInfo(int index, string fullName)
		{
			Index = index;
			FullName = fullName;
		}
	}
}
