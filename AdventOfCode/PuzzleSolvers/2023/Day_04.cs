namespace AdventOfCode.PuzzleSolvers._2023
{
    using System;
    using System.Text.RegularExpressions;
    using Logic.Extensions;
    using Logic.Modules;

    public class Day_04 : DayBase2023
    {
        public override int Day => 4;

        private List<string> input;

        [SetUp]
        public async Task SetUp()
        {
            this.input = await this.SplitInput();
        }

        [Test]
        public void PartOne()
        {
            var total = 0;
            
            foreach (var card in input)
            {
                var presentNumberCount = GetWinningCardCount(card);
                total += presentNumberCount > 0 ? (int)Math.Pow(2, presentNumberCount - 1) : 0;
            }

            total.Pass();
        }

        [Test]
        public void PartTwo()
        {
            var cardDict = new SafeDictionary<int, int>(_ => 1);
            
            for (var i = 0; i < this.input.Count; i++)
            {
                var card = this.input[i];
                var presentNumberCount = GetWinningCardCount(card);

                for (var j = 0; j < cardDict[i]; j++)
                {
                    for (var k = 0; k < presentNumberCount; k++)
                    {
                        if (k >= this.input.Count)
                        {
                            continue;
                        }

                        cardDict[i + k + 1] += 1;
                    }
                }
            }

            cardDict.Values.ToList().Sum().Pass();
        }

        private static int GetWinningCardCount(string cardInput)
        {
            var numberRegex = new Regex(@"\d+");

            var winningNumbers = numberRegex.Matches(cardInput.Split(':')[1].Split('|')[0]).Select(x => x.Value.ToInt()).ToList();
            var actualNumber = numberRegex.Matches(cardInput.Split(':')[1].Split('|')[1]).Select(x => x.Value.ToInt()).ToList();

            return actualNumber.Count(winningNumbers.Contains);
        }
    }
}
