namespace AdventOfCode.PuzzleSolvers._2022
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Logic;
	using NUnit.Framework;

	public class Day_10 : DayBase2022
    {
	    public override int Day => 10;

	    private List<string> Insctructions;

		[SetUp]
	    public async Task SetUp()
	    {
			this.Insctructions = await this.SplitInput();
		}

		[Test]
	    public void PartOne()
	    {
		    var register = 1;
		    var signalStrengths = new List<int>();
		    var registerInstructions = new Dictionary<int, int>();

		    for (var cycle = 1; cycle <= 220; cycle++)
		    {
			    var instruction = this.Insctructions[(cycle - 1) % this.Insctructions.Count];

			    var startingCycle = Math.Max(cycle, registerInstructions.Keys.LastOrDefault() + 1);
			    if (instruction.StartsWith("addx"))
			    {
				    var value = instruction.Split(" ")[1].ToInt();
				    registerInstructions.Add(startingCycle + 1, value);
			    }
			    else
			    {
					registerInstructions.Add(startingCycle, 0);
			    }

			    if ((cycle + 20) % 40 == 0)
			    {
					signalStrengths.Add(cycle * register);
			    }

			    if (registerInstructions.TryGetValue(cycle, out var addition))
			    {
				    register += addition;
			    }
		    }

			signalStrengths.Sum().Pass();
	    }

		[Test]
	    public void PartTwo()
	    {
		    var register = 1;
		    var lines = new List<List<string>>();
		    var registerInstructions = new Dictionary<int, int>();

		    for (var cycle = 0; cycle < 240; cycle++)
		    {
			    var instruction = this.Insctructions[cycle % this.Insctructions.Count];

			    var lineNumber = cycle / 40;
			    var linePosition = cycle % 40;

			    if (cycle % 40 == 0)
			    {
					lines.Add(new List<string>());
			    }

			    var startingCycle = Math.Max(cycle, registerInstructions.Keys.LastOrDefault() + 1);
			    if (instruction.StartsWith("addx"))
			    {
				    var value = instruction.Split(" ")[1].ToInt();
				    registerInstructions.Add(startingCycle + 1, value);
			    }
			    else
			    {
				    registerInstructions.Add(startingCycle, 0);
			    }

			    var newCharacter = Math.Abs(linePosition - register) < 2 ? "#" : " ";
				lines[lineNumber].Add(newCharacter);

				if (registerInstructions.TryGetValue(cycle + 1, out var addition))
				{
					register += addition;
				}
            }

			lines.Select(x => x.Join()).Join("\n").Pass();
	    }
    }
}
