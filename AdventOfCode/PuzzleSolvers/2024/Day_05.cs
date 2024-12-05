namespace AdventOfCode.PuzzleSolvers._2024
{
    using Logic.Extensions;

    [TestFixture]
    public class Day_05 : DayBase2024
    {
        public override int Day => 5;

        private List<(int before, int after)> rules;
        private List<List<int>> manuals;

        [SetUp]
        public async Task SetUp()
        {
            var input = await this.GetInput();

            var ruleSection = input.Split("\n\n")[0];
            var pagesSection = input.Split("\n\n")[1];

            this.rules = ruleSection.Split('\n').Select(x => (int.Parse(x.Split('|')[0]), int.Parse(x.Split('|')[1]))).ToList();
            this.manuals = pagesSection.Split('\n').Select(x => x.Split(',').Select(int.Parse).ToList()).ToList();
        }

        [Test]
        public void PartOne()
        {
            var pageNumberTotal = this.manuals.Sum(manual => CheckManualValidity(manual) ? manual[manual.Count / 2] : 0);
            pageNumberTotal.Pass();
        }

        [Test]
        public void PartTwo()
        {
            var incorrectManuals = this.manuals.Where(x => !CheckManualValidity(x)).ToList();
            var orderedManuals = incorrectManuals.Select(x => x.OrderBy(y => y, new RuleComparer(this.rules)).ToList());

            var pageNumberTotal = orderedManuals.Sum(x => x[x.Count / 2]);
            pageNumberTotal.Pass();
        }

        private bool CheckManualValidity(List<int> manual)
        {
            for (var i = 0; i < manual.Count; i++)
            {
                var page = manual[i];

                var rulesBefore = this.rules.Where(x => x.before == page);
                if (rulesBefore.Select(rule => manual.IndexOf(rule.after)).Any(index => index >= 0 && index < i))
                {
                    return false;
                }

                var rulesAfter = this.rules.Where(x => x.after == page);
                if (rulesAfter.Select(rule => manual.IndexOf(rule.before)).Any(index => index > i))
                {
                    return false;
                }
            }

            return true;
        }

        private class RuleComparer : IComparer<int>
        {
            private readonly List<(int before, int after)> rules;

            public RuleComparer(List<(int before, int after)> rules)
            {
                this.rules = rules;
            }

            public int Compare(int x, int y)
            {
                var rule = this.rules.SingleOrDefault(rule => 
                    (rule.before == x && rule.after == y) ||
                    (rule.before == y && rule.after == x));

                if (rule == default)
                {
                    return 0;
                }

                return rule.before == x ? -1 : 1;
            }
        }
    }
}
