namespace Massive.Samples.StatefulRollbacks
{
	public struct CollisionTree
	{
		public struct Node
		{
			public int Child1;
			public int Child2;
		}

		public ListPointer<Node> Nodes;
	}
}
