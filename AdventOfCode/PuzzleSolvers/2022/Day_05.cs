namespace AdventOfCode.PuzzleSolvers._2022
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using Logic;
	using NUnit.Framework;

	[TestFixture]
    public class Day_05 : DayBase2022
    {
	    public override int Day => 5;

	    private Dictionary<int, List<string>> crateConfiguration = new Dictionary<int, List<string>>();
	    private List<(int count, int from, int to)> commands;

		[SetUp]
	    public async Task SetUp()
	    {
		    var input = await this.SplitInput();

		    var plotWidthRow = input.ToList().IndexOf(input.First(c => c.StartsWith(" 1")));
		    var width = input[plotWidthRow].Trim().Split(' ').Last().ToInt();

		    foreach (var crateRow in input.Take(plotWidthRow))
		    {
				var row = crateRow.Replace("    ", " _");
			    var crates = row.Split(" ");

			    for (var i = 0; i < width; i++)
			    {
				    if (!crateConfiguration.ContainsKey(i))
				    {
						crateConfiguration.Add(i, new List<string>());
				    }

				    var crate = crates[i];
				    if (crate == "_")
				    {
					    continue;
				    }

				    crateConfiguration[i].Add(crate.TrimStart('[').TrimEnd(']'));
                }
		    }

			crateConfiguration.Keys.ToList().ForEach(key => crateConfiguration[key].Reverse());

			var numericRegex = new Regex("[0-9]+");
			commands = input.Skip(plotWidthRow + 2).Select(x => numericRegex.Matches(x))
				.Select(x => (x[0].Value.ToInt(), x[1].Value.ToInt() - 1, x[2].Value.ToInt() - 1)).ToList();
	    }

		[Test]
	    public override void PartOne()
	    {
		    foreach (var command in commands)
		    {
			    for (var i = 0; i < command.count; i++)
			    {
					crateConfiguration[command.to].Add(crateConfiguration[command.from].Last());
					crateConfiguration[command.from].RemoveAt(crateConfiguration[command.from].Count - 1);
			    }
		    }

		    string.Join("", crateConfiguration.Keys.Select(key => crateConfiguration[key].Last())).Pass();
	    }

		[Test]
	    public override void PartTwo()
	    {
		    foreach (var command in commands)
		    {
			    var from = crateConfiguration[command.from];

				crateConfiguration[command.to].AddRange(from.Skip(from.Count - command.count));
				crateConfiguration[command.from].RemoveRange(from.Count - command.count, command.count);
		    }

		    string.Join("", crateConfiguration.Keys.Select(key => crateConfiguration[key].Last())).Pass();
        }
    }
}
