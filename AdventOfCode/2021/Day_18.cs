namespace AdventOfCode._2021
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;

	[TestFixture]
	public class Day_18
	{
		private List<string> Numbers;

		[SetUp]
	    public void SetUp()
	    {
		    Numbers = Input.Split("\r\n").ToList();
	    }

		[Test]
	    public void PartOne()
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
	    public void PartTwo()
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
		    return this.ReduceNumber(combinedNumber, out root);
	    }

	    private string ReduceNumber(string input, out Node root)
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

	    private const string TestInput = @"[1,1]
[2,2]
[3,3]
[4,4]
[5,5]
[6,6]";

	    private const string TestInput2 = @"[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]
[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]
[[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]
[[[[2,4],7],[6,[0,5]]],[[[6,8],[2,8]],[[2,1],[4,5]]]]
[7,[5,[[3,8],[1,4]]]]
[[2,[2,2]],[8,[8,1]]]
[2,9]
[1,[[[9,3],9],[[9,0],[0,7]]]]
[[[5,[7,4]],7],1]
[[[[4,2],2],6],[8,7]]";

	    private const string TestInput3 = @"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]";

		private const string Input = @"[[8,[[8,6],[6,0]]],[8,[[1,8],1]]]
[[[8,[7,5]],[8,8]],[1,5]]
[[[4,[4,5]],[[0,8],7]],3]
[[[8,[5,9]],8],[[6,[1,5]],7]]
[[[5,[0,8]],[0,[4,6]]],[[7,[4,7]],[[8,8],4]]]
[[5,[[4,4],5]],[3,[7,6]]]
[[2,[[4,9],[1,4]]],[4,0]]
[3,[[[4,7],6],4]]
[[[[0,5],8],4],[0,3]]
[[[1,8],[4,[3,2]]],[4,[4,[7,5]]]]
[[0,7],2]
[[[6,4],[0,4]],[[[5,7],[8,6]],2]]
[[[1,9],6],[[0,7],[3,1]]]
[[[4,2],[[1,6],4]],3]
[[[[2,9],[6,2]],6],[[[0,0],[3,5]],[[2,0],1]]]
[5,[[[0,4],[5,8]],2]]
[[[7,6],4],[[[2,5],6],2]]
[[1,6],[9,[[6,2],5]]]
[[1,[[5,6],3]],[[[6,4],[9,9]],[[3,8],1]]]
[[[[8,0],8],[[1,2],7]],[[1,1],[[5,1],[3,8]]]]
[[[8,[2,9]],[[3,0],[1,9]]],[3,9]]
[[[[9,8],[4,9]],[9,7]],1]
[[1,6],[4,5]]
[[[9,1],3],5]
[[[5,[3,2]],9],[[1,[9,3]],[[1,5],1]]]
[[8,[5,[2,1]]],[[[6,6],7],[1,[3,9]]]]
[[[[4,5],2],5],[[2,4],[2,8]]]
[[[[2,2],7],6],[4,[[8,5],[2,6]]]]
[[[[0,8],[6,4]],[[0,4],[6,5]]],[[3,[2,4]],[[3,2],7]]]
[[[8,[3,7]],[[5,3],5]],[[[9,3],[3,4]],1]]
[[2,[1,0]],[[[8,8],[4,7]],[[8,2],0]]]
[7,[8,3]]
[[[6,1],[[9,6],[3,8]]],[[[5,5],[7,1]],[[6,0],4]]]
[[[4,[1,3]],[[1,1],0]],[7,[[8,8],9]]]
[1,[[3,0],7]]
[[[[3,0],5],[[3,7],2]],[[[5,0],2],[0,[4,9]]]]
[[[[4,1],[0,1]],3],[[[2,1],[3,3]],[6,[9,2]]]]
[[[1,7],[5,9]],[[1,[7,7]],[[3,9],0]]]
[[[3,[1,0]],[[1,1],4]],[[4,3],[4,[2,0]]]]
[[[[5,1],[6,2]],[[4,9],[2,0]]],[2,[7,2]]]
[[[3,[8,6]],8],[[[9,1],[0,9]],[8,7]]]
[8,[[9,7],[[6,9],9]]]
[1,[[4,7],[5,[9,4]]]]
[[4,[[4,8],[8,8]]],[[[4,2],0],[[4,4],6]]]
[[[[1,1],[7,2]],[9,[0,7]]],[3,3]]
[[[2,[1,2]],6],[[[0,4],0],[5,[5,7]]]]
[6,[3,[4,7]]]
[[[1,4],[[1,3],[4,2]]],[6,8]]
[8,[[[6,1],1],8]]
[2,[2,[5,0]]]
[[[[6,1],[1,1]],[[4,9],[3,8]]],[[6,[6,6]],[6,2]]]
[[2,4],[1,4]]
[[[[6,0],[7,7]],[[4,1],[4,8]]],[[[6,4],8],9]]
[[1,[[0,5],3]],7]
[[[[2,9],6],[[0,9],6]],[8,7]]
[[4,6],[[[5,3],6],0]]
[4,[[3,[5,2]],[5,6]]]
[[8,[[4,8],6]],[[[9,8],5],9]]
[[[[8,7],6],[1,[3,0]]],[[5,[5,3]],6]]
[[[[5,8],9],7],[[[7,9],[0,2]],[[6,4],0]]]
[6,[[1,8],[[5,6],7]]]
[[7,[[4,8],9]],8]
[[[9,[6,4]],[1,3]],[0,7]]
[[[[1,8],[5,3]],5],[[[5,8],8],[[3,0],5]]]
[[[3,[6,7]],[2,9]],[[[7,1],1],[2,[4,1]]]]
[[8,[[9,5],[4,0]]],[[[4,3],3],[[0,8],[3,1]]]]
[[3,9],[[[6,5],[1,4]],6]]
[[2,[2,[9,0]]],[4,[0,6]]]
[[5,[8,9]],[[[9,2],4],1]]
[[5,[[8,2],[6,0]]],[9,[8,8]]]
[[[[8,8],1],[[3,4],[8,1]]],4]
[[9,[[7,2],[9,8]]],[[2,[4,9]],[[2,9],5]]]
[[[3,[9,1]],2],[0,[3,[0,3]]]]
[[9,[2,[6,2]]],[9,[9,[0,0]]]]
[[[[2,6],[2,0]],5],[[[7,9],5],[[1,5],6]]]
[[1,[[6,3],1]],[[[4,1],[0,7]],2]]
[[[[0,9],2],[[8,5],9]],[[1,[1,7]],6]]
[[[0,[8,3]],3],[[[1,9],0],[[7,2],4]]]
[[[[2,2],5],[[1,6],5]],[[[4,8],2],[[3,2],[4,8]]]]
[[[8,[6,8]],6],[0,[[3,2],7]]]
[[7,[[2,0],9]],[[4,[2,4]],[[8,8],[4,5]]]]
[[4,[[5,2],6]],[[0,7],2]]
[[4,3],[[5,[2,1]],[8,[3,3]]]]
[[[[6,1],9],4],2]
[3,[[[8,0],[3,7]],[[2,9],[6,6]]]]
[6,[[3,3],[9,[3,6]]]]
[[[9,[9,4]],3],[[1,0],0]]
[[[[1,1],[4,5]],[8,1]],[8,[2,2]]]
[[6,[0,3]],0]
[[[[3,2],8],6],[[9,[0,6]],[5,6]]]
[[4,[[4,8],[2,5]]],[[8,8],[[9,9],3]]]
[[6,7],[[8,[9,1]],[[6,3],[3,5]]]]
[2,[[3,[0,7]],[[7,4],5]]]
[[[1,4],9],[1,[[1,4],[0,1]]]]
[[[8,[9,7]],7],[[8,4],[[5,2],[5,5]]]]
[[[[9,8],[0,0]],8],[[3,[7,4]],[[0,1],[3,9]]]]
[[[[2,3],[0,0]],0],[[0,1],[[4,9],0]]]
[[8,[[5,3],2]],[[[2,9],2],[2,0]]]
[[[[2,2],[6,1]],[2,[6,6]]],[5,0]]
[[7,3],[[1,5],[[8,7],[3,1]]]]";
    }
}
