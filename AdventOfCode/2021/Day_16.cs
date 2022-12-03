namespace AdventOfCode._2021
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Helpers;
	using NUnit.Framework;

	[TestFixture]
    public class Day_16
    {
	    private string binaryString;

	    [SetUp]
	    public void SetUp()
	    {
		    this.binaryString = string.Join(string.Empty,
			    Input.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
		}

		[Test]
	    public void PartOne()
	    {
		    var answer = ParsePackageOne(0, binaryString, out _);
			
		    Assert.AreEqual(860, answer);
		    Assert.Pass(answer.ToString());
	    }

	    [Test]
		public void PartTwo()
	    {
		    var answer = ParsePackageTwo(0, binaryString, out _);

			Assert.AreEqual(470949537659, answer.Single());
			Assert.Pass(answer.Single().ToString());
	    }

		private long ParsePackageOne(int pointer, string input, out int newPointer, bool continueScan = false)
		{
			var result = 0l;
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

			var result = 0l;
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

		private const string Input = @"A059141803C0008447E897180401F82F1E60D80021D11A3DC3F300470015786935BED80A5DB5002F69B4298A60FE73BE41968F48080328D00427BCD339CC7F431253838CCEFF4A943803D251B924EC283F16D400C9CDB3180213D2D542EC01092D77381A98DA89801D241705C80180960E93469801400F0A6CEA7617318732B08C67DA48C27551C00F972830052800B08550A277416401A5C913D0043D2CD125AC4B1DB50E0802059552912E9676931530046C0141007E3D4698E20008744D89509677DBF5759F38CDC594401093FC67BACDCE66B3C87380553E7127B88ECACAD96D98F8AC9E570C015C00B8E4E33AD33632938CEB4CD8C67890C01083B800E5CBDAB2BDDF65814C01299D7E34842E85801224D52DF9824D52DF981C4630047401400042E144698B2200C4328731CA6F9CBCA5FBB798021259B7B3BBC912803879CD67F6F5F78BB9CD6A77D42F1223005B8037600042E25C158FE0008747E8F50B276116C9A2730046801F29BC854A6BF4C65F64EB58DF77C018009D640086C318870A0C01D88105A0B9803310E2045C8CF3F4E7D7880484D0040001098B51DA0980021F17A3047899585004E79CE4ABD503005E610271ED4018899234B64F64588C0129EEDFD2EFBA75E0084CC659AF3457317069A509B97FB3531003254D080557A00CC8401F8791DA13080391EA39C739EFEE5394920C01098C735D51B004A7A92F6A0953D497B504F200F2BC01792FE9D64BFA739584774847CE26006A801AC05DE180184053E280104049D10111CA006300E962005A801E2007B80182007200792E00420051E400EF980192DC8471E259245100967FF7E6F2CF25DBFA8593108D342939595454802D79550C0068A72F0DC52A7D68003E99C863D5BC7A411EA37C229A86EBBC0CB802B331FDBED13BAB92080310265296AFA1EDE8AA64A0C02C9D49966195609C0594223005B80152977996D69EE7BD9CE4C1803978A7392ACE71DA448914C527FFE140";
    }
}
