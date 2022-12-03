namespace AdventOfCode.PuzzleSolvers._2021
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Helpers;
	using NUnit.Framework;

	[TestFixture]
    public class Day_16 : DayBase2021
    {
	    public override int Day => 16;

        private string binaryString;

	    [SetUp]
	    public async Task SetUp()
	    {
		    this.binaryString = string.Join(string.Empty,
			    (await this.GetInput()).Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
		}

	    [Test]
	    public override void PartOne()
	    {
		    var answer = ParsePackageOne(0, binaryString, out _);
			
		    Assert.AreEqual(860, answer);
		    Assert.Pass(answer.ToString());
	    }

	    [Test]
		public override void PartTwo()
	    {
		    var answer = ParsePackageTwo(0, binaryString, out _);

			Assert.AreEqual(470949537659, answer.Single());
			Assert.Pass(answer.Single().ToString());
	    }

		private long ParsePackageOne(int pointer, string input, out int newPointer, bool continueScan = false)
		{
			var result = 0L;
			var first = true;

			while ((first || continueScan) && input.Substring(pointer, input.Length - pointer).Contains("1"))
			{
				first = false;
				var (version, packageType) = GetPackageData(pointer, input, out pointer);
				result += version + (packageType == 4
					? ParseLiteralValue(pointer, input, false, out pointer)
					: ParseOperatorPackageOne(pointer, input, out pointer));
			}

			newPointer = pointer;

			return result;
		}

		private long ParseOperatorPackageOne(int pointer, string input, out int newPointer)
		{
			var lengthTypeId = input[pointer];
			pointer++;

			var result = 0L;
			if (lengthTypeId == '0')
			{
				var length = Convert.ToInt32(input.Substring(pointer, 15), 2);
				pointer += 15;
				result = ParsePackageOne(0, input.Substring(pointer, length), out _, true);
				pointer += length;
			}
			else
			{
				var amount = Convert.ToInt32(input.Substring(pointer, 11), 2);
				pointer += 11;
				for (var i = 0; i < amount; i++)
				{
					result += ParsePackageOne(pointer, input, out pointer);
				}
			}

			newPointer = pointer;
			return result;
		}

		private List<long> ParsePackageTwo(int pointer, string input, out int newPointer, bool continueScan = false)
		{
			var results = new List<long>();
			var first = true;

			while ((first || continueScan) && input.Substring(pointer, input.Length - pointer).Contains("1"))
			{
				first = false;
				var (_, packageType) = GetPackageData(pointer, input, out pointer);
				results.Add(packageType == 4
					? ParseLiteralValue(pointer, input, true, out pointer)
					: ParseOperatorPackageTwo(packageType, pointer, input, out pointer));
			}

			newPointer = pointer;
			return results;
		}

		private long ParseOperatorPackageTwo(int operatorType, int pointer, string input, out int newPointer)
		{
			var lengthTypeId = input[pointer];
			pointer++;

			var results = new List<long>();
			if (lengthTypeId == '0')
			{
				var length = Convert.ToInt32(input.Substring(pointer, 15), 2);
				pointer += 15;
				results = ParsePackageTwo(0, input.Substring(pointer, length), out _, true);
				pointer += length;
			}
			else
			{
				var amount = Convert.ToInt32(input.Substring(pointer, 11), 2);
				pointer += 11;
				for (var i = 0; i < amount; i++)
				{
					results.Add(ParsePackageTwo(pointer, input, out pointer).Single());
				}
			}

			newPointer = pointer;

			return operatorType switch
			{
				0 => results.Sum(),
				1 => results.Product(),
				2 => results.OrderBy(x => x).First(),
				3 => results.OrderBy(x => x).Last(),
				5 => results[0] > results[1] ? 1 : 0,
				6 => results[0] < results[1] ? 1 : 0,
				7 => results[0] == results[1] ? 1 : 0,
				_ => throw new ArgumentException()
			};
		}

		private static long ParseLiteralValue(int pointer, string input, bool returnGroupValue, out int newPointer)
		{
			var parsedGroup = "";
			var parsing = true;
			while (parsing)
			{
				var group = input.Substring(pointer, 5);
				parsing = group.First() == '1';

				parsedGroup += group.Substring(1, 4);
				pointer += 5;
			}

			newPointer = pointer;
			return returnGroupValue ? Convert.ToInt64(parsedGroup, 2) : 0;
		}

		private static (int version, int type) GetPackageData(int pointer, string data, out int newPointer)
		{
			var version = Convert.ToInt16(data.Substring(pointer, 3), 2);
			var packageType = Convert.ToInt16(data.Substring(pointer + 3, 3), 2);

			newPointer = pointer + 6;

			return (version, packageType);
		}
    }
}
