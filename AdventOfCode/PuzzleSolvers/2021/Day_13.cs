namespace AdventOfCode.PuzzleSolvers._2021
{
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
    public class Day_13 : DayBase2021
    {
	    public override int Day => 13;

        private static int gridWidth, gridHeight;

	    private List<Node> nodes;
	    private Dictionary<int, Node> nodeDict;
	    private List<FoldInstruction> foldInstructions;
		
		[SetUp]
	    public async Task SetUp()
	    {
		    var rows = (await this.GetInput()).Split("\n").ToList();

		    var coordinateRows = rows.Take(rows.IndexOf(rows.First(string.IsNullOrEmpty))).ToList();
		    var foldInstructionRows = rows.Skip(coordinateRows.Count + 1).Take(rows.Count);

		    var coordinates = coordinateRows
			    .Select(row => new Point(int.Parse(row.Split(",")[0]), int.Parse(row.Split(",")[1]))).ToList();

		    gridWidth = coordinates.MaxBy(x => x.X).X + 1;
		    gridHeight = coordinates.MaxBy(x => x.Y).Y + 1;

		    this.nodes = new List<Node>();

		    for (var y = 0; y < gridHeight; y++)
		    {
			    for (var x = 0; x < gridWidth; x++)
			    {
				    this.nodes.Add(new Node
					{
						PosX = x,
						PosY = y
					});
			    }
		    }

		    nodeDict = this.nodes.ToDictionary(x => x.Id);

			coordinates.Select(x => (x.Y * gridWidth) + x.X).ToList().ForEach(id => nodeDict[id].HasDot = true);

			this.foldInstructions = foldInstructionRows
				.Select(row => row.Replace("fold along ", string.Empty))
				.Select(row => new FoldInstruction
				{
					HorizontalFold = row.Contains("y"),
					Line = int.Parse(row.Substring(2, row.Length - 2))
				}).ToList();
	    }

	    [Test]
	    public override void PartOne()
	    {
		    ExecuteFoldInstruction(this.foldInstructions[0]);
		    var answer = this.nodes.Count(n => n.HasDot);

			Assert.Pass(answer.ToString());
	    }

	    [Test]
	    public override void PartTwo()
	    {
		    this.foldInstructions.ForEach(ExecuteFoldInstruction);
		    var answer = Visualize();

			Assert.Pass(answer);
	    }

	    private void ExecuteFoldInstruction(FoldInstruction instruction)
	    {
		    if (instruction.HorizontalFold)
		    {
			    this.FoldHorizontal(instruction.Line);
		    }
		    else
		    {
			    this.FoldVertical(instruction.Line);
		    }
		}

		private void FoldHorizontal(int line)
	    {
		    var dottedNodes = this.nodes.Where(x => x.HasDot && x.PosY > line);
		    foreach (var node in dottedNodes)
		    {
			    var distanceToLine = node.PosY - line;
			    this.nodeDict[(line - distanceToLine) * gridWidth + node.PosX].HasDot = true;
		    }

		    gridHeight = line;

			this.nodes = this.nodes.Where(x => x.PosY < line).ToList();
			this.nodeDict = this.nodes.ToDictionary(x => x.Id);
		}

	    private void FoldVertical(int line)
	    {
		    var dottedNodes = this.nodes.Where(x => x.HasDot && x.PosX > line);
		    foreach (var node in dottedNodes)
		    {
			    var distanceToLine = node.PosX - line;
			    this.nodeDict[node.PosY * gridWidth + line - distanceToLine].HasDot = true;
		    }

		    gridWidth = line;

		    this.nodes = this.nodes.Where(node => node.PosX < line).ToList();
		    this.nodeDict = this.nodes.ToDictionary(x => x.Id);
	    }

	    private string Visualize()
	    {
		    var visualization = "";
		    for (var i = 0; i < gridHeight; i++)
		    {
			    for (var j = 0; j < gridWidth; j++)
			    {
				    visualization += this.nodeDict[i * gridWidth + j].HasDot ? "#" : " ";
			    }

			    visualization += "\r\n";
		    }

		    return visualization;
	    }

		private class Node
	    {
		    internal int Id => this.PosY * gridWidth + this.PosX;
			internal int PosX { get; set; }
			internal int PosY { get; set; }
			internal bool HasDot { get; set; }
	    }

	    private class FoldInstruction
	    {
		    internal int Line { get; set; }
			internal bool HorizontalFold { get; set; }
	    }
    }
}
