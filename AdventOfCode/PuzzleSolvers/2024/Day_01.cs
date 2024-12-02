namespace AdventOfCode.PuzzleSolvers._2024
{
    using Logic.Extensions;
    using System;

    [TestFixture]
    public class Day_01 : DayBase2024
    {
        public override int Day => 1;

        private List<int> inputLeft, inputRight;

        [SetUp]
        public async Task SetUp()
        {
            var input = await this.SplitInput();
            
            this.inputLeft = new();
            this.inputRight = new();

            foreach (var line in input)
            {
                inputLeft.Add(int.Parse(line.Split("   ")[0]));
                inputRight.Add(int.Parse(line.Split("   ")[1]));
            }

            this.inputLeft = this.inputLeft.OrderBy(x => x).ToList();
            this.inputRight = this.inputRight.OrderBy(x => x).ToList();
        }

        [Test]
        public void PartOne()
        {
            var answer = this.inputLeft.Select((x, i) => Math.Abs(x - this.inputRight[i])).Sum();
            answer.Pass();
        }

        [Test]
        public void PartTwo()
        {
            var answer = this.inputLeft.Sum(x => x * this.inputRight.Count(y => x == y));
            answer.Pass();
        }
    }
}
