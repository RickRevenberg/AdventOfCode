namespace AdventOfCode.PuzzleSolvers._2023
{
    using System.Text.RegularExpressions;
    using Logic.Extensions;

    public class Day_06 : DayBase2023
    {
        public override int Day => 6;

        private List<string> input;

        [SetUp]
        public async Task SetUp()
        {
            this.input = await this.SplitInput();
        }

        [Test]
        public void PartOne()
        {
            var parsedInput = ParseInput();
            this.Solve(parsedInput);
        }

        [Test]
        public void PartTwo()
        {
            this.input = this.input.Select(x => x.Replace(" ", "")).ToList();
            
            var parsedInput = ParseInput();
            this.Solve(parsedInput);
        }

        private List<(long time, long record)> ParseInput()
        {
            var numberRegex = new Regex(@"\d+");

            var splitInput = this.input.Select(x => numberRegex.Matches(x).Select(y => y.Value.ToLong()).ToList()).ToList();
            return splitInput[0].Select((x, i) => (x, splitInput[1][i])).ToList();
        }

        private void Solve(List<(long time, long record)> parsedInput)
        {
            var total = 1L;

            foreach (var (time, record) in parsedInput)
            {
                for (var i = 0; i <= time; i++)
                {
                    var length = i * (time - i);
                    if (length > record)
                    {
                        total *= (time - (i * 2) + 1);
                        break;
                    }
                }
            }

            total.Pass();
        }
    }
}
