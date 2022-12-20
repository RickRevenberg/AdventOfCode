namespace AdventOfCode.PuzzleSolvers._2021
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Helpers;
	using NUnit.Framework;

	[TestFixture]
    public class Day_14 : DayBase2021
    {
	    public override int Day => 14;

        private string currentPolymer;
	    private Dictionary<string, (string, string)> InsertionMap;

		[SetUp]
	    public async Task SetUp()
	    {
		    var rows = (await this.GetInput()).Split("\n").ToList();
		    currentPolymer = rows.First();

		    var insertionRules = new Dictionary<string, string>();

		    rows.Skip(2)
			    .Select(x => (x.Split(" -> ")[0], x.Split(" -> ")[1])).ToList()
			    .ForEach(pairing => insertionRules.Add(pairing.Item1, pairing.Item2));

		    InsertionMap = insertionRules.Keys
			    .Select(key => (key, ($"{key.ToCharArray()[0]}{insertionRules[key]}", $"{insertionRules[key]}{key.ToCharArray()[1]}")))
			    .ToDictionary(x => x.key, x => x.Item2);
	    }

		[Test]
	    public void PartOne()
	    {
		    Solution(10);
	    }

		[Test]
	    public void PartTwo()
	    {
		    Solution(40);
	    }

        private void Solution(int iterations)
	    {
			var pairs = ExpandPolymer(currentPolymer, iterations);
			pairs.Add(currentPolymer.ToCharArray().Last().ToString(), 1);

			var elements = new SafeDictionary<string, long>();

			foreach (var key in pairs.Keys)
			{
				var element = key.ToCharArray()[0].ToString();
				elements[element] += pairs[key];
			}

			var mostCommonElement = elements.Keys.OrderBy(key => elements[key]).Last();
            var leastCommonElement = elements.Keys.OrderBy(key => elements[key]).First();

            var answer = elements[mostCommonElement] - elements[leastCommonElement];

			Assert.Pass(answer.ToString());
		}

	    private Dictionary<string, long> ExpandPolymer(string input, int iterations)
	    {
		    var pairs = new SafeDictionary<string, long>();
		    var splitInput = input.ToCharArray().Select(x => x.ToString()).ToList();

		    for (var i = 0; i < splitInput.Count - 1; i++)
		    {
			    var pair = $"{splitInput[i]}{splitInput[i + 1]}";
			    pairs[pair]++;
		    }

		    for (var i = 0; i < iterations; i++)
		    {
			    var newPairs = new SafeDictionary<string, long>();

			    foreach (var key in pairs.Keys)
			    {
				    var (item1, item2) = InsertionMap[key];
				    
				    newPairs[item1] += pairs[key];
				    newPairs[item2] += pairs[key];
			    }

			    pairs = newPairs;
		    }

		    return pairs;
	    }
    }
}
