namespace AdventOfCode.PuzzleSolvers._2023
{
    using Logic.Extensions;

    public class Day_09 : DayBase2023
    {
        public override int Day => 9;

        private List<List<int>> input;

        [SetUp]
        public async Task SetUp()
        {
            this.input = (await this.SplitInput()).Select(x => x.Split(' ').Select(y => y.ToInt()).ToList()).ToList();
        }

        [Test]
        public void PartOne()
        {
            var values = this.input.Select(FindNextInSequence).ToList();
            values.Select(x => x.next).Sum().Pass();
        }

        [Test]
        public void PartTwo()
        {
            var values = this.input.Select(FindNextInSequence).ToList();
            values.Sum(x => x.previous).Pass();
        }

        private (int previous, int next) FindNextInSequence(List<int> values)
        {
            var sequences = new List<List<int>> { values };
            while (sequences.Last().Any(x => x != 0))
            {
                var nextSequence = sequences.Last().Skip(1).Select((x, i) => x - sequences.Last()[i]).ToList();
                sequences.Add(nextSequence);
            }

            sequences.Reverse();
            var (previousModifier, nextModifier) = (sequences[1].First(), sequences[1].First());

            for (var i = 2; i < sequences.Count; i++)
            {
                previousModifier = sequences[i].First() - previousModifier;
                nextModifier = sequences[i].Last() + nextModifier;
            }

            return (previousModifier, nextModifier);
        }
    }
}
