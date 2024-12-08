namespace Massive
{
	public struct IdentifierInfo
	{
		public readonly int Index;
		public readonly string FullName;

		public IdentifierInfo(int index, string fullName)
		{
			Index = index;
			FullName = fullName;
		}
	}
}
