namespace AdventOfCode.PuzzleSolvers._2022
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Logic;
	using NUnit.Framework;

	public class Day_08 : DayBase2022
    {
	    public override int Day => 8;

	    private List<List<int>> formattedInput;

	    [SetUp]
	    public async Task SetUp()
	    {
			this.formattedInput = (await this.SplitInput()).Select(x => x.Select(y => y.ToString().ToInt()).ToList()).ToList();
		}

		[Test]
	    public void PartOne()
	    {
		    var visibleTrees = formattedInput[0].Count * 2 + (formattedInput.Count * 2) - 4;

		    for (var i = 1; i < formattedInput.Count - 1; i++)
		    {
			    var line = formattedInput[i];
			    
                for (var j = 1; j < line.Count - 1; j++)
			    {
				    var column = formattedInput.Select(x => x[j]).ToList();

                    var treeHeight = line[j];

				    var heighestLeft = line.Take(j).Max();
				    var heighestRight = line.Skip(j + 1).Max();
				    var heighestUp = column.Take(i).Max();
				    var heighestDown = column.Skip(i + 1).Max();

                    if (treeHeight > new List<int>{ heighestLeft, heighestRight, heighestUp, heighestDown }.Min())
				    {
					    visibleTrees++;
				    }
			    }
		    }

		    visibleTrees.Pass();
	    }

		[Test]
	    public void PartTwo()
	    {
		    var highestScore = -1;

		    for (var i = 1; i < formattedInput.Count - 1; i++)
		    {
			    for (var j = 1; j < formattedInput.Count - 1; j++)
			    {
				    var treeHeight = formattedInput[i][j];

				    var line = formattedInput[i];
				    var column = formattedInput.Select(x => x[j]).ToList();

				    var lDistance = CalculateDistance(line.Take(j).Reverse().ToList(), treeHeight);
				    var rDistance = CalculateDistance(line.Skip(j + 1).ToList(), treeHeight);
				    var uDistance = CalculateDistance(column.Take(i).Reverse().ToList(), treeHeight);
				    var dDistance = CalculateDistance(column.Skip(i + 1).ToList(), treeHeight);

				    var result = lDistance * rDistance * uDistance * dDistance;

				    highestScore = Math.Max(highestScore, result);
			    }
			}

			highestScore.Pass();
		}

	    private static int CalculateDistance(List<int> orderedData, int checkHeight)
	    {
		    return !orderedData.Any() ? 0 :
			    orderedData.Max() < checkHeight ? orderedData.Count : orderedData.FindIndex(x => x >= checkHeight) + 1;
	    }
    }
}
