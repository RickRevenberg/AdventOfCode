namespace AdventOfCode._2021
{
	using System.Collections.Generic;
	using System.Linq;
	using Helpers;
	using NUnit.Framework;
	using NUnit.Framework.Internal.Builders;

	[TestFixture]
    public class Day_14
    {
	    private string currentPolymer;
	    private Dictionary<string, (string, string)> InsertionMap;

		[SetUp]
	    public void SetUp()
	    {
		    var rows = Input.Split("\r\n").ToList();
		    currentPolymer = rows.First();

		    var insertionRules = new Dictionary<string, string>();

		    rows.Skip(2)
			    .Select(x => (x.Split(" -> ")[0], x.Split(" -> ")[1])).ToList()
			    .ForEach(pairing => insertionRules.Add(pairing.Item1, pairing.Item2));

		    InsertionMap = insertionRules.Keys
			    .Select(key => (key, ($"{key.ToCharArray()[0]}{insertionRules[key]}", $"{insertionRules[key]}{key.ToCharArray()[1]}")))
			    .ToDictionary(x => x.key, x => x.Item2);
	    }

		[TestCase(10, TestName = "PartOne")]
		[TestCase(40, TestName = "PartTwo")]
		public void Solution(int iterations)
	    {
			var pairs = ExpandPolymer(currentPolymer, iterations);
			pairs.Add(currentPolymer.ToCharArray().Last().ToString(), 1);

			var elements = new SafeDictionary<string, long>();

			foreach (var key in pairs.Keys)
			{
				var element = key.ToCharArray()[0].ToString();
				elements[element] += pairs[key];
			}

			var mostCommonElement = elements.Keys.OrderBy(key => elements[key]).Last();
            var leastCommonElement = elements.Keys.OrderBy(key => elements[key]).First();

            var answer = elements[mostCommonElement] - elements[leastCommonElement];

			Assert.Pass(answer.ToString());
		}

	    private Dictionary<string, long> ExpandPolymer(string input, int iterations)
	    {
		    var pairs = new SafeDictionary<string, long>();
		    var splitInput = input.ToCharArray().Select(x => x.ToString()).ToList();

		    for (var i = 0; i < splitInput.Count - 1; i++)
		    {
			    var pair = $"{splitInput[i]}{splitInput[i + 1]}";
			    pairs[pair]++;
		    }

		    for (var i = 0; i < iterations; i++)
		    {
			    var newPairs = new SafeDictionary<string, long>();

			    foreach (var key in pairs.Keys)
			    {
				    var (item1, item2) = InsertionMap[key];
				    
				    newPairs[item1] += pairs[key];
				    newPairs[item2] += pairs[key];
			    }

			    pairs = newPairs;
		    }

		    return pairs;
	    }

	    private const string TestInput = @"NNCB

CH -> B
HH -> N
CB -> H
NH -> C
HB -> C
HC -> B
HN -> C
NN -> C
BH -> H
NC -> B
NB -> B
BN -> B
BB -> N
BC -> B
CC -> N
CN -> C";
	    private const string Input = @"FNFPPNKPPHSOKFFHOFOC

VS -> B
SV -> C
PP -> N
NS -> N
BC -> N
PB -> F
BK -> P
NV -> V
KF -> C
KS -> C
PV -> N
NF -> S
PK -> F
SC -> F
KN -> K
PN -> K
OH -> F
PS -> P
FN -> O
OP -> B
FO -> C
HS -> F
VO -> C
OS -> B
PF -> V
SB -> V
KO -> O
SK -> N
KB -> F
KH -> C
CC -> B
CS -> C
OF -> C
FS -> B
FP -> H
VN -> O
NB -> N
BS -> H
PC -> H
OO -> F
BF -> O
HC -> P
BH -> S
NP -> P
FB -> C
CB -> H
BO -> C
NN -> V
SF -> N
FC -> F
KK -> C
CN -> N
BV -> F
FK -> C
CF -> F
VV -> B
VF -> S
CK -> C
OV -> P
NC -> N
SS -> F
NK -> V
HN -> O
ON -> P
FH -> O
OB -> H
SH -> H
NH -> V
FF -> B
HP -> B
PO -> P
HB -> H
CH -> N
SN -> P
HK -> P
FV -> H
SO -> O
VH -> V
BP -> V
CV -> P
KP -> K
VB -> N
HV -> K
SP -> N
HO -> P
CP -> H
VC -> N
CO -> S
BN -> H
NO -> B
HF -> O
VP -> K
KV -> H
KC -> F
HH -> C
BB -> K
VK -> P
OK -> C
OC -> C
PH -> H";
    }
}
