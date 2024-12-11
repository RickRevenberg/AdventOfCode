namespace AdventOfCode.PuzzleSolvers._2024
{
    using Logic.Extensions;
    using System;

    [TestFixture]
    public class Day_07 : DayBase2024
    {
        public override int Day => 7;

        private List<Equation> Equations = [];

        [SetUp]
        public async Task SetUp()
        {
            var input = await this.SplitInput();
            foreach (var data in input)
            {
                var total = long.Parse(data.Split(':')[0]);
                var parts = data.Split(':')[1].Split(' ')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(long.Parse).ToList();

                Equations.Add(new Equation { Total = total, Parts = parts});
            }
        }

        [Test]
        public void PartOne()
        {
            var usedOperators = new List<Operator> { Operator.Add, Operator.Multiply };
            
            SolveValidEquations(usedOperators).Pass();
        }

        [Test]
        public void PartTwo()
        {
            var usedOperators = new List<Operator> { Operator.Add, Operator.Multiply, Operator.Concatenation };
            
            SolveValidEquations(usedOperators).Pass();
        }

        private long SolveValidEquations(List<Operator> usedOperators)
        {
            var maxLength = this.Equations.Select(x => x.Parts.Count - 1).Max();
            var permutationDict = new Dictionary<int, List<List<Operator>>>();

            for (var i = 1; i <= maxLength; i++)
            {
                permutationDict.Add(i, GetOperationPermutations(usedOperators, i));
            }

            var validTotal = 0L;

            foreach (var equation in this.Equations)
            {
                var positions = equation.Parts.Count - 1;
                var permutations = permutationDict[positions];

                foreach (var permutation in permutations)
                {
                    var total = equation.Parts[0];

                    for (var i = 1; i < equation.Parts.Count; i++)
                    {
                        var func = GetOperatorFunc(permutation[i - 1]);
                        total = func(total, equation.Parts[i]);
                    }

                    if (total == equation.Total)
                    {
                        validTotal += total;
                        break;
                    }
                }
            }

            return validTotal;
        }

        private class Equation
        {
            internal long Total { get; init; }

            internal List<long> Parts { get; init; } = [];
        }

        private enum Operator
        {
            Add,
            Multiply,
            Concatenation
        }

        private static Func<long, long, long> GetOperatorFunc(Operator op)
        {
            return op switch
            {
                Operator.Add => (a, b) => a + b,
                Operator.Multiply => (a, b) => a * b,
                Operator.Concatenation => (a, b) => long.Parse($"{a}{b}"),
                _ => throw new InvalidOperationException()
            };
        }

        private static List<List<Operator>> GetOperationPermutations(List<Operator> usedOperators, int length)
        {
            if (length <= 1)
            {
                return usedOperators.Select(x => new List<Operator> { x }).ToList();
            }

            var permutations = new List<List<Operator>>();

            var subPermutations = GetOperationPermutations(usedOperators, length - 1);

            foreach (var sub in subPermutations)
            {
                foreach (var op in usedOperators)
                {
                    var newPermutation = new List<Operator> { op };
                    newPermutation.AddRange(sub);

                    permutations.Add(newPermutation);
                }
            }

            return permutations;
        }
    }
}
