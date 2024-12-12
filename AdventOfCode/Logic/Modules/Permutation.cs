namespace AdventOfCode.Logic.Modules
{
    using System.Collections.Generic;

    internal static class Permutation
    {
        internal static List<List<T>> CalculatePermutations<T>(this List<T> options, int? maxLength = null)
        {
            var usedMaxLength = maxLength ?? options.Count;
            if (usedMaxLength <= 1)
            {
                return options.Select(x => new List<T> { x }).ToList();
            }

            var permutations = new List<List<T>>();
            var subPermutations = options.CalculatePermutations(usedMaxLength - 1);

            foreach (var sub in subPermutations)
            {
                foreach (var op in options)
                {
                    var newPermutation = new List<T> { op };
                    newPermutation.AddRange(sub);

                    permutations.Add(newPermutation);
                }
            }

            return permutations;
        }
    }
}
