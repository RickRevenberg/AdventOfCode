namespace AdventOfCode.PuzzleSolvers._2023
{
    using Logic.Extensions;
    using Logic.Modules;
    using System;
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
        public void TestOne()
        {
            var parsedInput = this.input.Select(x => (x.Split(' ')[0], x.Split(' ')[1].Split(',').Select(int.Parse).ToList())).ToList();
            var groupData = parsedInput.Select(x => (x.Item2.Count, DeterminePossibleSolutions(x))).ToList();
            var unmatchingData = groupData.Where(x => x.Item2 == int.MaxValue).ToList();

            //this.DetermineTotalArrangements(5);
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

        private static int DeterminePossibleSolutions((string conditions, List<int> counts) input)
        {
            var dataGroups = input.conditions.Split('.').Where(x => x != "").ToList();
            var certainCountPlacements = DetermineDefinitiveGroupPlacements(dataGroups, input);

            if (certainCountPlacements.Values.SelectMany(x => x).Count() == input.counts.Count)
            {
                return SolveDefinitiveCountPlacement(dataGroups,
                    certainCountPlacements.Keys.Select(key => (key, certainCountPlacements[key]
                        .Select(value => input.counts[value]).ToList())).ToDictionary(x => x.key, x => x.Item2));
            }

            return -1; // dataGroups.Count;
        }

        private static SafeDictionary<int, List<int>> DetermineDefinitiveGroupPlacements(List<string> dataGroups, (string conditions, List<int> counts) input)
        {
            var validPermutations = 0;
            Dictionary<int, (int min, int max)> groupLimitCache = dataGroups.Select((x, i) => (i, x))
                .ToDictionary(x => x.i, x => (x.x.ToCharArray().Any(y => y == '#') ? 1 : 0, x.x.Length));

            var certainCountPlacements = new SafeDictionary<int, List<int>>(defaultValue: c => []);

            if (dataGroups.Count == 1)
            {
                certainCountPlacements[0] = input.counts.Select((x, i) => i).ToList();
                return certainCountPlacements;
            }

            if (groupLimitCache[0].min > 0)
            {
                certainCountPlacements[0].Add(0);
                groupLimitCache[0] = (groupLimitCache[0].min, groupLimitCache[0].max - (input.counts[0] + 1));
            }

            if (groupLimitCache[dataGroups.Count - 1].min > 0)
            {
                certainCountPlacements[dataGroups.Count - 1].Add(input.counts.Count - 1);
                groupLimitCache[dataGroups.Count - 1] = (groupLimitCache[dataGroups.Count - 1].min, groupLimitCache[dataGroups.Count - 1].max - (input.counts.Last() + 1));
            }

            var changes = true;
            while (changes)
            {
                changes = false;

                var undesignatedCountIndexes = input.counts.Select((x, i) => (i, x)).Where(x => !certainCountPlacements.Values.SelectMany(y => y).Contains(x.i));

                foreach (var count in undesignatedCountIndexes)
                {
                    var fittingGroups = dataGroups.Select((x, i) => (i, x)).Where(x => groupLimitCache[x.i].max >= count.x).ToList();
                    if (fittingGroups.Count == 1)
                    {
                        changes = true;

                        certainCountPlacements[fittingGroups.Single().i].Add(count.i);

                        var cacheValue = groupLimitCache[fittingGroups.Single().i];
                        groupLimitCache[fittingGroups.Single().i] = (cacheValue.min, Math.Max(0, cacheValue.max - (count.x + 1)));
                    }
                }
            }

            return certainCountPlacements;
        }

        private static int SolveDefinitiveCountPlacement(List<string> groups, Dictionary<int, List<int>> placements)
        {
            var permutationsPerGroup = new List<int>();
            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];

                var adjustedGroupLength = group.Length - placements[i].Select(x => x - 1).Sum();
                permutationsPerGroup.Add(SolvePossiblePermutations(adjustedGroupLength, placements[i].Count));
            }

            return (int) permutationsPerGroup.Product();
        }

        private static int SolvePossiblePermutations(int groupLength, int itemCount)
        {
            if (itemCount <= 1)
            {
                return groupLength;
            }

            var minGroupLength = itemCount + (itemCount - 1);
            if (groupLength == minGroupLength)
            {
                return 1;
            }

            var total = 0;
            var possibleStartingPositions = groupLength - minGroupLength;
            for (var i = 0; i < possibleStartingPositions; i++)
            {
                total += SolvePossiblePermutations(groupLength - 2 - i, itemCount - 1);
            }

            return total;
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
