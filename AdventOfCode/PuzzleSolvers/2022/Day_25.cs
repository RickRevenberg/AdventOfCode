namespace AdventOfCode.PuzzleSolvers._2022
{
    using Logic.Extensions;
    using Logic.Modules;
    using System;

    public class Day_25 : DayBase2022
    {
        private List<string> snafuNumbers;

        private string TestInput => @"1=-0-2
12111
2=0=
21
2=01
111
20012
112
1=-1=
1-12
12
1=
122".Replace("\r", "");

        public override int Day => 25;

        [SetUp]
        public async Task SetUp()
        {
            var input = await this.GetInput();
            this.snafuNumbers = input.Split("\n").ToList();
        }

        [Test]
        public void PartOne()
        {
            var results = this.snafuNumbers.Select(SnafuToLong).ToList();
            var snafu = LongToSnafu(results.Sum());

            snafu.Pass();
        }

        [Test]
        public void PartTwo()
        {
            "You win!".Pass();
        }

        private static long SnafuToLong(string snafu)
        {
            long result = 0;
            var chars = snafu.ToArray();

            for (var i = 0; i < chars.Length; i++)
            {
                var power = (long)(chars.Length - i - 1);
                var multiplier = chars[i] switch
                {
                    '=' => -2,
                    '-' => -1,
                    _ => int.Parse(chars[i].ToString())
                };

                result += (long)(Math.Pow(5l, power) * multiplier);
            }

            return result;
        }

        private static string LongToSnafu(long input)
        {
            var powerCache = new Dictionary<int, long>();
            for (var i = 0; i < int.MaxValue; i++)
            {
                var value = (long)Math.Pow(5, i);
                powerCache.Add(i, value);

                if (value > input)
                {
                    break;
                }
            }

            var snafuValues = new SafeDictionary<int, int>();
            for (var i = powerCache.Keys.Max() - 1; i >= 0; i--)
            {
                var result = (int)(input / powerCache[i]);
                snafuValues[i] = result;
                input -= result * powerCache[i];
            }

            while (snafuValues.Values.Any(x => x < -2 || x > 2))
            {
                for (var i = 0; i < snafuValues.Keys.Count; i++)
                {
                    if (snafuValues[i] > 2)
                    {
                        var diff = 5 - snafuValues[i];
                        snafuValues[i] = -diff;

                        snafuValues[i + 1] += 1;
                    }
                    else if (snafuValues[i] < -2)
                    {
                        var diff = snafuValues[i] - -5;
                        snafuValues[i] = -diff;

                        snafuValues[i + 1] -= 1;
                    }
                }
            }

            return string.Join("", snafuValues.Values.Select(x =>
            {
                return x switch
                {
                    -2 => "=",
                    -1 => "-",
                    _ => x.ToString()
                };
            }));
        }
    }
}
