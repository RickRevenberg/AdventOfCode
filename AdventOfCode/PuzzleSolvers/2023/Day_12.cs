namespace AdventOfCode.PuzzleSolvers._2023
{
    using Logic.Extensions;
    using System.Linq;

    public class Day_12 : DayBase2023
    {
        public override int Day => 12;

        private List<string> input;

        [SetUp]
        public async Task SetUp()
        {
            this.input = await this.SplitInput();
        }

        [Test]
        public void PartOne()
        {
            var totalArrangements = 0;
            List<(List<char> condition, string counts)> parsedData = this.input.Select(x => (x.Split(' ')[0].ToCharArray().ToList(), x.Split(' ')[1])).ToList();

            foreach (var data in parsedData)
            {
                var permutations = GetPossiblePermutations(data.condition);
                totalArrangements += permutations.Where(x => GetConditionScore(x) == data.counts).ToList().Count;
            }

            totalArrangements.Pass();
        }

        [Test]
        public void PartTwo()
        {
            this.DetermineTotalArrangements(5);
        }

        private void DetermineTotalArrangements(int multiplier)
        {
            List<(char[] condition, List<int> description)> parsedData = this.input.Select(x => (
                        x.Split(' ')[0].ToCharArray(),
                        x.Split(' ')[1].Split(',').Select(x => int.Parse(x.ToString())).ToList()))
                .ToList();

            var totalArrangements = 0;

            foreach (var data in parsedData)
            {
                var nextStartIndex = 0;
                var indexes = new List<int>();
                for (var i = 0; i < data.description.Count; i++)
                {
                    for (var j = nextStartIndex; j < data.condition.Length; j++)
                    {
                        var subString = data.condition.Skip(nextStartIndex).Take(data.description[i] + 1).Join("");
                        if (CanContainDigit(subString, data.description[i]))
                        {
                            indexes.Add(j);
                            break;
                        }
                    }

                    nextStartIndex = indexes.Last() + data.description[i] + 1;
                }
            }
        }

        private List<List<char>> GetPossiblePermutations(List<char> input)
        {
            var permutations = new List<List<char>>();

            var index = input.FindIndex(x => x == '?');
            if (index >= 0)
            {
                var permA = input.Clone();
                var permB = input.Clone();

                permA[index] = '.';
                permB[index] = '#';

                permutations.Add(permA);
                permutations.Add(permB);

                permutations.AddRange(GetPossiblePermutations(permA));
                permutations.AddRange(GetPossiblePermutations(permB));
            }

            return permutations.Where(x => x.IndexOf('?') == -1).ToList();
        }

        private string GetConditionScore(List<char> input)
        {
            var data = new List<int>();
            var index = input.FindIndex(x => x == '#');

            while (index >= 0)
            {
                var nextIndex = input.FindIndex(index, x => x == '.');
                if (nextIndex == -1)
                {
                    data.Add(input.Count - index);
                    break;
                }

                data.Add(nextIndex - index);

                index = input.FindIndex(nextIndex, x => x == '#');
            }

            return data.Join(",");
        }

        // #..#, 2
        private bool CanContainDigit(string substring, int length)
        {
            var positionCount = substring.Count(x => x == '#' || x == '?');
            if (positionCount < length)
            {
                return false;
            }

            var groups = substring.Split('.');
            return groups.Any(x => x.Length >= length);
        }

        protected override string TestInput()
        {
            return @"???.### 1,1,3
.??..??...?##. 1,1,3
?#?#?#?#?#?#?#? 1,3,1,6
????.#...#... 4,1,1
????.######..#####. 1,6,5
?###???????? 3,2,1";
        }
    }
}
