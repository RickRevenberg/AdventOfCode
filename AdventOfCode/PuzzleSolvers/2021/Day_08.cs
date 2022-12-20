namespace AdventOfCode.PuzzleSolvers._2021
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
    public class Day_08 : DayBase2021
    {
	    public override int Day => 8;

        private List<InputRecord> Records;

	    private readonly Dictionary<int, string[]> CorrectDisplays = new Dictionary<int, string[]>
	    {
		    {0, new []{"a", "b", "c", "e", "f", "g"}},
		    {1, new []{"c", "f"}},
		    {2, new []{"a", "c", "d", "e", "g"}},
		    {3, new []{"a", "c", "d", "f", "g"}},
		    {4, new []{"b", "c", "d", "f"}},
		    {5, new []{"a", "b", "d", "f", "g"}},
		    {6, new []{"a", "b", "d", "e", "f", "g"}},
		    {7, new []{"a", "c", "f"}},
		    {8, new []{"a", "b", "c", "d", "e", "f", "g"}},
		    {9, new []{"a", "b", "c", "d", "f", "g"}}
	    };

	    [SetUp]
	    public async Task SetUp()
	    {
		    var inputs = (await this.GetInput()).Split("\n");
		    Records = inputs.Select(x => new InputRecord
		    {
			    DisplayData = x.Split(" | ")[0].Split(" ").ToList(),
				NotedData = x.Split(" | ")[1].Split(" ").ToList()
		    }).ToList();
	    }

	    [Test]
		public void PartOne()
		{
			var uniqueNumbers = Records.Sum(record =>
				record.NotedData.Count(n => n.Length == 2 || n.Length == 3 || n.Length == 4 || n.Length == 7));

			Assert.Pass(uniqueNumbers.ToString());
		}

		[Test]
		public void PartTwo()
		{
			var resolvedNumbers = new List<int>();

			foreach (var record in Records)
			{
				// Ignore eights and duplicates. they're useless.
				record.DisplayData = record.DisplayData.Distinct().Where(x => x.Length != 7).ToList();

				var options = new Dictionary<string, List<string>>
				{
					{"a", new List<string> {"a", "b", "c", "d", "e", "f", "g"}},
					{"b", new List<string> {"a", "b", "c", "d", "e", "f", "g"}},
					{"c", new List<string> {"a", "b", "c", "d", "e", "f", "g"}},
					{"d", new List<string> {"a", "b", "c", "d", "e", "f", "g"}},
					{"e", new List<string> {"a", "b", "c", "d", "e", "f", "g"}},
					{"f", new List<string> {"a", "b", "c", "d", "e", "f", "g"}},
					{"g", new List<string> {"a", "b", "c", "d", "e", "f", "g"}}
				};

				void Strike(int digit, string number)
				{
					var correctSegments = CorrectDisplays[digit];
					number.ToCharArray()
						.Select(x => x.ToString())
						.ToList()
						.ForEach(letter => options[letter] = options[letter].Where(x => correctSegments.Contains(x)).ToList());

					options.Keys.ToList()
						.ForEach(key =>
						{
							if (options[key].Count == 1)
							{
								options.Keys
									.Where(k => k != key).ToList()
									.ForEach(k => options[k] = options[k].Where(x => x != options[key][0]).ToList());
							}
						});
				}

				// 1st gen
				var one = record.DisplayData.Single(x => x.Length == 2);
				var seven = record.DisplayData.Single(x => x.Length == 3);
				var four = record.DisplayData.Single(x => x.Length == 4);

				// 2nd gen
				var three = record.DisplayData
					.Where(x => x.Length == 5)
					.Single(x => MatchingSegments(seven, x) == 3);

				var nine = record.DisplayData
					.Where(x => x.Length == 6)
					.Single(x => MatchingSegments(four, x) == 4);

				var six = record.DisplayData
					.Where(x => x.Length == 6)
					.Single(x => MatchingSegments(x, one) == 1);

				// 3rd gen
				var two = record.DisplayData
					.Where(x => x.Length == 5)
					.Single(x => MatchingSegments(x, nine) == 4);

				var five = record.DisplayData
					.Where(x => x.Length == 5)
					.Single(x => MatchingSegments(x, nine) == 5 && MatchingSegments(x, one) == 1);

				var zero = record.DisplayData
					.Where(x => x.Length == 6)
					.Single(x => MatchingSegments(x, two) == 4 && MatchingSegments(x, five) == 4);


				Strike(1, one);
				Strike(2, two);
				Strike(3, three);
				Strike(4, four);
				Strike(5, five);
				Strike(6, six);
				Strike(7, seven);
				Strike(9, nine);
				Strike(0, zero);

				var decodedNumber = "";

				record.NotedData.ForEach(x =>
				{
					x = string.Join("", x.ToCharArray().Select(y => options[y.ToString()].Single()));

					var number = CorrectDisplays.Keys.Single(key =>
						CorrectDisplays[key].Length == x.Length &&
						MatchingSegments(x, string.Join("", CorrectDisplays[key])) == x.Length);

					decodedNumber += number;
				});

				resolvedNumbers.Add(Convert.ToInt32(decodedNumber));
			}

			var total = resolvedNumbers.Sum();

			Assert.Pass(total.ToString());
		}

		private static int MatchingSegments(string one, string two)
		{
			var allSegments = new List<string> { "a", "b", "c", "d", "e", "f", "g" };

			return allSegments.Count(x => one.Contains(x) && two.Contains(x));
		}

	    private class InputRecord
	    {
		    internal List<string> DisplayData;
		    internal List<string> NotedData;
	    }
    }
}
