namespace AdventOfCode.PuzzleSolvers._2024
{
    using Logic.Extensions;
    using System.Text.RegularExpressions;

    [TestFixture]
    public class Day_03 : DayBase2024
    {
        public override int Day => 3;

        private string memoryString;

        [SetUp]
        public async Task SetUp()
        {
            this.memoryString = await this.GetInput();
        }

        [Test]
        public void PartOne()
        {
            var answer = CalculateTotal(this.memoryString);
            answer.Pass();
        }

        [Test]
        public void PartTwo()
        {
            var blocks = this.memoryString.Split("do")
                .Select(x => (!x.StartsWith("n't()"), x))
                .ToList();

            var newString = blocks.Where(x => x.Item1).Select(x => x.x).Join("");
            var answer = CalculateTotal(newString);

            answer.Pass();
        }

        private static int CalculateTotal(string input)
        {
            var commandRegex = new Regex("mul\\(([0-9]+,[0-9]+)\\)");
            var matches = commandRegex.Matches(input);

            return matches
                .Select(x => x.Value.Substring(4, x.Length - 5).Split(','))
                .Select(x => (int.Parse(x[0]), int.Parse(x[1])))
                .Sum(x => (x.Item1 * x.Item2));
        }
    }
}
