namespace AdventOfCode.PuzzleSolvers._2024
{
    using Logic.Extensions;
    using Logic.Modules;
    using System;

    public class Day_11 : DayBase2024
    {
        public override int Day => 11;

        private List<long> stones;

        private const int IterationBlockCount = 5;

        [SetUp]
        public async Task SetUp()
        {
            this.stones = (await this.SplitInput())[0].Split(" ").Select(long.Parse).ToList(); 
        }

        [Test]
        public void PartOne()
        {
            var answer = Solve(25, this.stones);
            answer.Pass();
        }

        [Test]
        public void PartTwo()
        {
            var answer = Solve(75, this.stones);
            answer.Pass();
        }

        private static long Solve(int amountOfBlinks, List<long> stones)
        {
            if (amountOfBlinks % IterationBlockCount != 0)
            {
                throw new InvalidOperationException();
            }

            var checkDigits = stones;
            var stonePresenceCount = new SafeDictionary<long, long>();
            var calculationCache = new SafeDictionary<long, List<(long number, long amount)>>();

            for (var i = 0; i < amountOfBlinks / IterationBlockCount; i++)
            {
                var newData = new List<(long input, List<(long result, long amount)>)>();

                foreach (var digit in checkDigits)
                {
                    if (calculationCache[digit] != null)
                    {
                        newData.Add((digit, calculationCache[digit]));
                        continue;
                    }

                    var result = RunIterations(IterationBlockCount, digit);
                    var groupedResult = result.GroupBy(x => x, (d, a) => (d, (long)a.Count())).ToList();
                    newData.Add((digit, groupedResult));
                }

                newData.ForEach(x => calculationCache[x.input] = x.Item2);
                checkDigits.Clear();
                checkDigits = newData.SelectMany(x => x.Item2.Select(y => y.result)).Distinct().ToList();

                var totalStones = newData.SelectMany(x => x.Item2.Select(y => (y.result, y.amount * Math.Max(1, stonePresenceCount[x.input])))).ToList();
                stonePresenceCount.Clear();
                totalStones.ForEach(x => stonePresenceCount[x.result] += x.Item2);
            }

            return stonePresenceCount.Values.Sum(x => (long)x);
        }

        private static List<long> RunIterations(int amountOfBlinks, long digit)
        {
            var stones = new List<long> { digit };

            for (var i = 0; i < amountOfBlinks; i++)
            {
                var addedStones = new List<long>();

                for (var j = 0; j < stones.Count; j++)
                {
                    var value = stones[j];
                    var (count, divisor) = GetDigitCount(value);

                    if (value == 0)
                    {
                        stones[j] = 1;
                    }
                    else if (count % 2 == 0)
                    {
                        var left = value / divisor;
                        var right = value % divisor;

                        stones[j] = -1;

                        addedStones.Add(left);
                        addedStones.Add(right);
                    }
                    else
                    {
                        stones[j] = value * 2024;
                    }
                }

                stones.RemoveAll(x => x == -1);
                stones.AddRange(addedStones);
            }

            return stones;
        }

        // Looks clunky, but performs the best.
        private static (int count, long divisor) GetDigitCount(long input)
        {
            return input switch
            {
                < 10L => (1, 1),
                < 100L => (2, 10),
                < 1000L => (3, 10),
                < 10000L => (4, 100),
                < 100000L => (5, 100),
                < 1000000L => (6, 1000),
                < 10000000L => (7, 1000),
                < 100000000L => (8, 10000),
                < 1000000000L => (9, 10000),
                < 10000000000L => (10, 100000),
                < 100000000000L => (11, 100000),
                < 1000000000000L => (12, 1000000),
                < 10000000000000L => (13, 1000000),
                < 100000000000000L => (14, 10000000),
                < 1000000000000000L => (15, 10000000),
                < 10000000000000000L => (16, 100000000),
                < 100000000000000000L => (17, 100000000),
                < 1000000000000000000L => (18, 1000000000),
                _ => (19, 1000000000)
            };
        }
    }
}
