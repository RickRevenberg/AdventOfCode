namespace AdventOfCode.PuzzleSolvers._2023
{
    using System;
    using System.Text.RegularExpressions;
    using Logic.Extensions;
    using Logic.Modules;

    public class Day_05 : DayBase2023
    {
        public override int Day => 5;

        private List<string> input;

        private readonly Regex NumberRegex = new Regex(@"\d+");

        [SetUp]
        public async Task SetUp()
        {
            this.input = await this.SplitInput();
        }

        [Test]
        public void PartOne()
        {
            var rangeNumbers = this.NumberRegex.Matches(this.input[0]).Select(x => x.Value.ToLong()).ToList();
            var ranges = rangeNumbers.Select(x => (x, 1l)).ToList();

            Solve(ranges);
        }

        [Test]
        public void PartTwo()
        {
            var rangeNumbers = this.NumberRegex.Matches(this.input[0]).Select(x => x.Value.ToLong()).ToList();
            var ranges = new List<(long start, long length)>();
            for (var i = 0; i < rangeNumbers.Count; i += 2)
            {
                ranges.Add((rangeNumbers[i], rangeNumbers[i + 1]));
            }

            Solve(ranges);
        }

        private void Solve(List<(long start, long length)> ranges)
        {
            var splitInput = this.input.Split(x => x == "");

            List<List<(long start, long end, long diff)>> structuredInstructions = splitInput.Skip(1).Select(x => x.Skip(2).Select(input =>
            {
                var values = this.NumberRegex.Matches(input).Select(x => x.Value.ToLong()).ToList();
                var (source, dest, range) = (values[1], values[0], values[2]);

                return (source, (source + range - 1), dest - source);
            }).OrderBy(x => x.source).ToList()).ToList();

            var map = ParseInstructionsToMap(structuredInstructions);
            var routes = MergeMapLayers(map);
            var bestLocation = DetermineOptimalLocation(routes, ranges);

            bestLocation.Pass();
        }

        private static List<SafeDictionary<long, long>> ParseInstructionsToMap(
            List<List<(long start, long end, long diff)>> instructions)
        {
            var dicts = new List<SafeDictionary<long, long>>();

            foreach (var instruction in instructions)
            {
                dicts.Add(new SafeDictionary<long, long>
                {
                    { 0, 0 }
                });

                foreach (var (start, end, diff) in instruction)
                {
                    var dictKeys = dicts.Last().Keys.OrderBy(x => x).ToList();
                    var currentUpperValueKey = dictKeys.Last(x => x <= end + 1);
                    var currentUpperValue = dicts.Last()[currentUpperValueKey];

                    var previousKey = dictKeys.Last(x => x <= start);

                    dicts.Last()[start] = dicts.Last()[previousKey] + diff;

                    dictKeys.Where(x => x > start && x <= end).ToList().ForEach(x =>
                    {
                        dicts.Last()[x] += diff;
                    });

                    dicts.Last()[end + 1] = currentUpperValue;
                }
            }

            return dicts;
        }

        private static SafeDictionary<long, long> MergeMapLayers(List<SafeDictionary<long, long>> map)
        {
            var root = map[0];

            for (var i = 1; i < map.Count; i++)
            {
                var rootKeys = root.Keys.OrderBy(x => x).ToList();
                var compareKeys = map[i].Keys.OrderBy(x => x).ToList();

                for (var j = 0; j < rootKeys.Count; j++)
                {
                    var key = rootKeys[j];

                    var keyDiff = root[key];
                    var keyRange = rootKeys.FirstOrDefault(x => x > key) - key;
                    keyRange = keyRange > 0 ? keyRange : long.MaxValue;

                    var nextApplicableKey = compareKeys.Last(x => x <= key + keyDiff);
                    var nextKeyRange = compareKeys.FirstOrDefault(x => x > nextApplicableKey) - nextApplicableKey;
                    nextKeyRange = nextKeyRange > 0 ? nextKeyRange : long.MaxValue;

                    var positionDiff = (key + keyDiff) - nextApplicableKey;

                    if (nextKeyRange == long.MaxValue || nextKeyRange >= keyRange + positionDiff)
                    {
                        root[key] += map[i][nextApplicableKey];
                        continue;
                    }

                    var maxApplicableRange = nextKeyRange - positionDiff;

                    root[key + maxApplicableRange] = root[key];
                    root[key] += map[i][nextApplicableKey];

                    rootKeys = root.Keys.OrderBy(x => x).ToList();
                }

                map[i] = null;
            }

            return root;
        }

        private static long DetermineOptimalLocation(SafeDictionary<long, long> routes, List<(long start, long length)> ranges)
        {
            var bestLocation = long.MaxValue;
            var routeKeys = routes.Keys.OrderBy(x => x).ToList();

            for (var i = 0; i < routeKeys.Count; i++)
            {
                var isLast = i == routeKeys.Count - 1;
                var key = routeKeys[i];

                foreach (var range in ranges)
                {
                    if (key > (range.start + range.length - 1) || (!isLast && routeKeys[i + 1] <= range.start))
                    {
                        continue;
                    }

                    var bestPosition = Math.Max(range.start, key);

                    var location = routes[key] + bestPosition;
                    bestLocation = location < bestLocation ? location : bestLocation;
                }
            }

            return bestLocation;
        }
    }
}
