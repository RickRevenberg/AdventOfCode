namespace AdventOfCode.PuzzleSolvers._2023
{
    using Logic.Extensions;

    public class Day_07 : DayBase2023
    {
        public override int Day => 7;

        private List<(string hand, int bid, HandRank rank)> parsedInput;

        [SetUp]
        public async Task SetUp()
        {
            var input = await this.SplitInput();
            this.parsedInput = input
                .Select(x => (x.Split(' ')[0], x.Split(' ')[1].ToInt(), DetermineRank(x.Split(' ')[0])))

                // Parse card letters to be in alphabetical order.
                .Select(x => (x.Item1.Replace("T", "B").Replace("J", "C").Replace("Q", "D").Replace("K", "E").Replace("A", "F"), x.Item2, x.Item3))
                .ToList();
        }

        [Test]
        public void PartOne()
        {
            this.Solve();
        }

        [Test]
        public void PartTwo()
        {
            this.parsedInput = this.parsedInput
                .Select(x => (x.hand.Replace("C", "1"), x.bid, DetermineRank(x.hand.Replace("C", "1")))).ToList();

            this.Solve();
        }

        private void Solve()
        {
            var groupedCards = this.parsedInput
                .OrderBy(x => x.rank)
                .GroupBy(x => x.rank).ToDictionary(x => x.Key, x => x.OrderBy(x => x).ToList());

            var total = 0;
            var tracker = 1;

            foreach (var group in groupedCards.Values)
            {
                foreach (var card in group)
                {
                    total += (tracker * card.bid);
                    tracker++;
                }
            }

            total.Pass();
        }

        private HandRank DetermineRank(string hand)
        {
            var cards = hand.ToCharArray();
            var jokerCount = cards.Count(x => x == '1');
            cards = cards.Where(x => x != '1').ToArray();

            var distinctCards = cards.Distinct().ToList();

            if (distinctCards.Count <= 1)
            {
                return HandRank.FiveOfAKind;
            }

            if (distinctCards.Count == 2)
            {
                var cardA = distinctCards.First();
                var cardB = distinctCards.Last();

                var cardACount = cards.Count(x => x == cardA);
                var cardBCount = cards.Count(x => x == cardB);

                return (cardACount > 1 && cardBCount > 1) ? HandRank.FullHouse : HandRank.FourOfAKind;
            }

            if (distinctCards.Count == 3)
            {
                return jokerCount > 0 || (distinctCards.Any(x => cards.Count(y => x == y) == 3))
                    ? HandRank.ThreeOfAKind : HandRank.TwoPair;
            }

            return (jokerCount > 0 || distinctCards.Count == 4) ? HandRank.OnePair : HandRank.HighCard;
        }

        private enum HandRank
        {
            HighCard = 0,
            OnePair = 1,
            TwoPair = 2,
            ThreeOfAKind = 3,
            FullHouse = 4,
            FourOfAKind = 5,
            FiveOfAKind = 6
        }
    }
}
