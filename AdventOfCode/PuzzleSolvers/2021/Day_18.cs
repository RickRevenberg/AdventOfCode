namespace AdventOfCode.PuzzleSolvers._2021
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
	public class Day_18 : DayBase2021
	{
		public override int Day => 18;

        private List<string> Numbers;

		[SetUp]
	    public async Task SetUp()
	    {
		    Numbers = (await this.GetInput()).Split("\n").ToList();
	    }

	    [Test]
	    public override void PartOne()
	    {
		    var value = Numbers.First();
		    Node finalRoot = null;

		    for (var i = 1; i < Numbers.Count; i++)
		    {
			    value = AddNumbers(value, Numbers[i], out finalRoot);
		    }

		    var answer = GetTreeMagnitude(finalRoot);
			Assert.Pass(answer.ToString());
	    }

		[Test]
	    public override void PartTwo()
	    {
			var largest = -1;

			for (var i = 0; i < Numbers.Count; i++)
			{
				for (var j = 0; j < Numbers.Count; j++)
				{
					if (i == j)
					{
						continue;
					}

					AddNumbers(Numbers[i], Numbers[j], out var root);
					var magnitude = GetTreeMagnitude(root);

					largest = Math.Max(largest, magnitude);
				}
			}

			Assert.Pass(largest.ToString());
	    }

	    private string AddNumbers(string one, string two, out Node root)
	    {
		    var combinedNumber = $"[{one},{two}]";
		    return ReduceNumber(combinedNumber, out root);
	    }

	    private static string ReduceNumber(string input, out Node root)
	    {
		    root = new Node { Root = true, Depth = 0 };
		    CreateTree(root, input);
			ReduceTree(root);

			return ConvertTreeToString(root);
	    }

	    private static void CreateTree(Node point, string input)
	    {
		    if (int.TryParse(input, out var value))
		    {
			    point.Value = value;
			    return;
		    }

		    input = input.Substring(1, input.Length - 2);

			var splitIndex = -1;
		    int opening = 0, closing = 0;
		    for (var i = 0; i < input.Length; i++)
		    {
			    if (input[i].Equals('['))
			    {
				    opening++;
			    }
				else if (input[i].Equals(']'))
			    {
				    closing++;
			    }
				else if (opening == closing && input[i].Equals(','))
			    {
				    splitIndex = i;
				    break;
			    }
		    }

		    var leftChild = input.Substring(0, splitIndex);
		    var rightChild = input.Substring(splitIndex + 1, input.Length - splitIndex - 1);

		    var leftNode = new Node { Parent = point, Depth = point.Depth + 1};
		    var rightNode = new Node { Parent = point, Depth = point.Depth + 1 };

		    point.LeftChild = leftNode;
		    point.RightChild = rightNode;

			CreateTree(leftNode, leftChild);
			CreateTree(rightNode, rightChild);
	    }

	    private static void ReduceTree(Node root)
	    {
		    List<Node> OrderNodes(Node point)
		    {
			    if (point == null)
			    {
				    return null;
			    }
			    if (point.Value != null)
			    {
				    return new List<Node>{ point };
			    }

			    return new List<List<Node>>
			    {
				    OrderNodes(point.LeftChild),
				    OrderNodes(point.RightChild)
			    }.SelectMany(x => x).Where(x => x != null).ToList();
		    }

		    var changed = true;
		    while (changed)
		    {
			    changed = false;
			    var orderedNodes = OrderNodes(root);
			    
			    for (var i = 0; i < orderedNodes.Count; i++)
			    {
				    var checkedNode = orderedNodes[i];
					if (checkedNode.Depth >= 5)
					{
						var sibling = orderedNodes.SingleOrDefault(n => n != checkedNode && n.Parent == checkedNode.Parent);

						if (i != 0)
						{
							orderedNodes[i - 1].Value += checkedNode.Value;
						}

						if (i != orderedNodes.Count - 2 && sibling != null)
						{
							orderedNodes[i + 2].Value += sibling.Value;
						}

						checkedNode.Parent.LeftChild = null;
						checkedNode.Parent.RightChild = null;
						checkedNode.Parent.Value = 0;

						changed = true;
						break;
					}
			    }

			    if (changed)
			    {
				    continue;
			    }

			    for (var i = 0; i < orderedNodes.Count; i++)
			    {
				    var checkedNode = orderedNodes[i];
					if (checkedNode.Value > 9)
				    {
					    var leftNode = new Node
					    {
						    Parent = checkedNode,
						    Depth = checkedNode.Depth + 1,
						    Value = checkedNode.Value / 2
					    };

					    var rightNode = new Node
					    {
						    Parent = checkedNode,
						    Depth = checkedNode.Depth + 1,
						    Value = (checkedNode.Value + 1) / 2
					    };

					    checkedNode.LeftChild = leftNode;
					    checkedNode.RightChild = rightNode;
					    checkedNode.Value = null;

					    changed = true;
					    break;
				    }
				}
		    }
	    }

	    private static string ConvertTreeToString(Node root)
	    {
		    if (root.Value != null)
		    {
			    return root.Value.ToString();
		    }

		    return $"[{ConvertTreeToString(root.LeftChild)},{ConvertTreeToString(root.RightChild)}]";
	    }

	    private static int GetTreeMagnitude(Node root)
	    {
		    var leftValue = root.LeftChild.Value ?? GetTreeMagnitude(root.LeftChild);
		    var rightValue = root.RightChild.Value ?? GetTreeMagnitude(root.RightChild);

		    return leftValue * 3 + rightValue * 2;
	    }

	    private class Node
	    {
			internal bool Root { get; set; }
			internal int? Value { get; set; }
			internal int Depth { get; set; }

			internal Node Parent { get; set; }
			internal Node LeftChild { get; set; }
			internal Node RightChild { get; set; }

			public override string ToString()
			{
				return $"{this.Depth}: {this.Value}";
			}
	    }
	}
}
