namespace AdventOfCode.PuzzleSolvers._2024
{
    using Logic.Extensions;
    using Logic.Modules;
    using System;
    using System.Text.RegularExpressions;

    public class Day_13 : DayBase2024
    {
        public override int Day => 13;

        private List<ArcadeMachine> machines;

        [SetUp]
        public async Task SetUp()
        {
            var input = await this.SplitInput();
            this.machines = new List<ArcadeMachine>();

            for (var i = 0; i < input.Count; i += 4)
            {
                var aDelta = Regex.Matches(input[i], "\\+[0-9]+").Select(x => int.Parse(x.Value[1..])).ToList();
                var bDelta = Regex.Matches(input[i + 1], "\\+[0-9]+").Select(x => int.Parse(x.Value[1..])).ToList();
                var prizeLoc = Regex.Matches(input[i + 2], "=[0-9]+").Select(x => int.Parse(x.Value[1..])).ToList();

                this.machines.Add(new ()
                {
                    ButtonADelta = (aDelta[0], aDelta[1]),
                    ButtonBDelta = (bDelta[0], bDelta[1]),
                    Prize = (prizeLoc[0], prizeLoc[1])
                });
            }
        }

        [Test]
        public void PartOne()
        {
            var totalCost = machines.Select(QuickMath).Sum();
            totalCost.Pass();
        }

        [Test]
        public void PartTwo()
        {
            machines.ForEach(machine =>
            {
                machine.Prize = (machine.Prize.X + 10000000000000, machine.Prize.Y + 10000000000000);
            });

            var totalCost = machines.Select(QuickMath).Sum();
            totalCost.Pass();
        }

        private static long QuickMath(ArcadeMachine machine)
        {
            var reversed = false;

            var vectorA = new Vector2D((0, 0), machine.ButtonADelta);
            var vectorB = new Vector2D((0, 0), machine.ButtonBDelta);
            var prizeVector = new Vector2D((0, 0), machine.Prize);

            var angleA = vectorA.Angle();
            var angleB = vectorB.Angle();
            var anglePrize = prizeVector.Angle();

            if (angleB > angleA)
            {
                (angleA, angleB) = (angleB, angleA);
                reversed = true;
            }

            var internalAngleA = angleA - anglePrize;
            var internalAngleB = 180 - (90 - (90 - angleA)) + angleB;
            var internalAnglePrize = (180 - internalAngleA) - internalAngleB;

            var sideLengthPrize = prizeVector.Length();

            var sineConstant = sideLengthPrize / Math.Sin(internalAngleB * (Math.PI / 180));

            var sideLengthA = sineConstant * Math.Sin(internalAnglePrize * (Math.PI / 180));
            var sideLengthB = sineConstant * Math.Sin(internalAngleA * (Math.PI / 180));

            if (reversed)
            {
                (sideLengthA, sideLengthB) = (sideLengthB, sideLengthA);
            }

            var vectorALength = vectorA.Length();
            var vectorBLength = vectorB.Length();

            var divisorA = sideLengthA / vectorALength;
            var divisorB = sideLengthB / vectorBLength;

            const double checkPrecision = 0.01;

            if (Math.Abs(Math.Round(divisorA) - divisorA) < checkPrecision && Math.Abs(Math.Round(divisorB) - divisorB) < checkPrecision)
            {
                var a = (long)(divisorA + checkPrecision);
                var b = (long)(divisorB + checkPrecision);

                return (a * 3) + b;
            }

            return 0;
        }

        private class ArcadeMachine
        {
            internal (long X, long Y) Prize;

            internal (int X, int Y) ButtonADelta, ButtonBDelta;
        }
    }
}
