namespace AdventOfCode.PuzzleSolvers._2024
{
    using Logic.Extensions;
    using System;

    [TestFixture]
    public class Day_02 : DayBase2024
    {
        public override int Day => 2;

        private List<List<int>> Reports;

        [SetUp]
        public async Task SetUp()
        {
            var input = await this.SplitInput();
            this.Reports = input.Select(x => x.Split(' ').Select(int.Parse).ToList()).ToList();
        }

        [Test]
        public void PartOne()
        {
            var answer = this.Reports.Sum(x => IsReportSafe(x) ? 1 : 0);
            answer.Pass();
        }

        [Test]
        public void PartTwo()
        {
            var safeCount = 0;
            foreach (var report in this.Reports)
            {
                if (IsReportSafe(report))
                {
                    safeCount++;
                    continue;
                }

                for (var i = 0; i < report.Count; i++)
                {
                    var newReport = report.Clone();
                    newReport.RemoveAt(i);

                    if (IsReportSafe(newReport))
                    {
                        safeCount++;
                        break;
                    }
                }
            }

            safeCount.Pass();
        }

        private static bool IsReportSafe(List<int> report)
        {
            for (var i = 1; i < report.Count; i++)
            {
                var diff = Math.Abs(report[i] - report[i - 1]);
                if (diff < 1 || diff > 3)
                {
                    return false;
                }
            }

            var reportString = string.Join("", report);
            return reportString == string.Join("", report.OrderBy(x => x)) ||
                   reportString == string.Join("", report.OrderByDescending(x => x));
        }
    }
}
