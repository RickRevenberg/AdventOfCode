namespace AdventOfCode.PuzzleSolvers._2023
{
    using System;
    using System.Text.RegularExpressions;
    using Logic.Extensions;

    public class Day_02 : DayBase2023
    {
        public override int Day => 2;

        private List<string> input;

        [SetUp]
        public async Task SetUp()
        {
            this.input = await this.SplitInput();
        }

        [Test]
        public void PartOne()
        {
            const int maxRed = 12;
            const int maxGreen = 13;
            const int maxBlue = 14;

            var totalPossible = 0;
            for (var i = 0; i < this.input.Count; i++)
            {
                var sets = this.input[i].Split(';');
                var possible = sets.All(set =>
                {
                    var (red, green, blue) = ParseSet(set);
                    return red <= maxRed && green <= maxGreen && blue <= maxBlue;
                });

                totalPossible += possible ? (i + 1) : 0;
            }

            totalPossible.Pass();
        }

        [Test]
        public void PartTwo()
        {
            var totalPower = 0;
            foreach (var sets in this.input.Select(line => line.Split(';')))
            {
                var (currRed, currGreen, currBlue) = (0, 0, 0);
                foreach (var set in sets)
                {
                    var (red, green, blue) = ParseSet(set);

                    currRed = Math.Max(red, currRed);
                    currGreen = Math.Max(green, currGreen);
                    currBlue = Math.Max(blue, currBlue);
                }

                totalPower += (currRed * currGreen * currBlue);
            }

            totalPower.Pass();
        }

        private static (int red, int blue, int green) ParseSet(string set)
        {
            var redRegex = new Regex(@"\d* r");
            var greenRegex = new Regex(@"\d* g");
            var blueRegex = new Regex(@"\d* b");

            var red = redRegex.Matches(set).FirstOrDefault()?.ToString().Split(' ')[0].ToInt() ?? 0;
            var green = greenRegex.Matches(set).FirstOrDefault()?.ToString().Split(' ')[0].ToInt() ?? 0;
            var blue = blueRegex.Matches(set).FirstOrDefault()?.ToString().Split(' ')[0].ToInt() ?? 0;

            return (red, green, blue);
        }
    }
}
