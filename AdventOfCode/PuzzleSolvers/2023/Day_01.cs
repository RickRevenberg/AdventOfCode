namespace AdventOfCode.PuzzleSolvers._2023
{
    using System.Text.RegularExpressions;
    using Logic.Extensions;

    public class Day_01 : DayBase2023
    {
	    public override int Day => 1;

        private string input;
        private List<string> splitInput;

		[SetUp]
	    public async Task SetUp()
        {
            this.input = await this.GetInput();
            this.splitInput = await this.SplitInput();
	    }

        // 55447
	    [Test]
	    public void PartOne()
        {
            var numbers = new List<int>();
            foreach (var line in this.splitInput)
            {
                var number = Regex.Matches(line, @"\d");
                var combinedNumber = string.Join("", new List<Match> { number.First(), number.Last() });

				numbers.Add(int.Parse(combinedNumber));
            }

			numbers.Sum().Pass();
        }

        // 54706
	    [Test]
	    public async Task PartTwo()
        {
            this.input = this.input
                .Replace("one", "o1e")
                .Replace("two", "t2o")
                .Replace("three", "th3ee")
                .Replace("four", "f4ur")
                .Replace("five", "f5ve")
                .Replace("six", "s6x")
                .Replace("seven", "se7en")
                .Replace("eight", "ei8ht")
                .Replace("nine", "n9ne");

            this.splitInput = await this.SplitInput();

            PartOne();
        }

        protected override string TestInput()
        {
            return this.input;
        }
    }
}
