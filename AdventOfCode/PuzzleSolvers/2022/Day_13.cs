namespace AdventOfCode.PuzzleSolvers._2022
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using AdventOfCode.Logic.Extensions;
    using Logic;
    using NUnit.Framework;

    public class Day_13 : DayBase2022
	{
		public override int Day => 13;

		private List<Packet> packets = new();

		private bool testing = false;

		[SetUp]
		public async Task SetUp()
		{
			var input = testing
				? TestInput.Split("\r\n").ToList()
				: await this.SplitInput();

			this.packets = input.Where(x => x != "").Select(ParsePacket).ToList();
		}

		[Test] // 6187
		public void PartOne()
		{
			var answer = 0;
			for (var i = 0; i < packets.Count; i += 2)
			{
				var left = packets[i];
				var right = packets[i + 1];

				if (ComparePacketOrder(left, right)!.Value)
				{
					answer += ((i / 2) + 1);
				}
			}

			answer.Pass();
		}

		[Test]
		public void PartTwo()
		{
			packets.AddRange(new List<Packet>
			{
				new Packet
				{
					Id = 2,
					Packets = new Dictionary<int, Packet>
					{
						{ 0, new Packet { Integers = new Dictionary<int, int> { { 0, 2 } } } }
					}
				},
				new Packet
				{
					Id = 6,
					Packets = new Dictionary<int, Packet>
					{
						{ 0, new Packet { Integers = new Dictionary<int, int> { { 0, 6 } } } }
					}
				}
			});

			while (true)
			{
				var orderChanged = false;

				for (var i = 1; i < this.packets.Count; i++)
				{
					if (ComparePacketOrder(this.packets[i - 1], this.packets[i]) == false)
					{
						(this.packets[i - 1], this.packets[i]) = (this.packets[i], this.packets[i - 1]);
						orderChanged = true;
					}
				}

				if (!orderChanged)
				{
					break;
				}
			}

			var twoDividerIndex = this.packets.FindIndex(x => x.Id == 2) + 1;
			var sixDividerIndex = this.packets.FindIndex(x => x.Id == 6) + 1;

			(twoDividerIndex * sixDividerIndex).Pass();
		}

		private static Packet ParsePacket(string input)
		{
			var numbersRegex = new Regex("^([0-9]|,)+");

			input = input.Substring(1, input.Length - 2);
			var packet = new Packet();

			while (input != "")
			{
				foreach (var number in numbersRegex.Matches(input).SelectMany(x => x.Value.Split(",")).Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToInt()))
				{
					packet.Integers.Add(packet.NextKey, number);
				}

				var nestedPacketStartIndex = input.IndexOf("[");
				if (nestedPacketStartIndex >= 0)
				{
					input = input.Substring(nestedPacketStartIndex, input.Length - nestedPacketStartIndex);

					var tracker = 0;
					var nestedPacketEndIndex = input
						.ToCharArray().ToList()
						.FindIndex(c =>
						{
							tracker += c == '[' ? 1 : 0;
							tracker -= c == ']' ? 1 : 0;

							return tracker == 0;
						});

					packet.Packets.Add(packet.NextKey, ParsePacket(input.Substring(0, nestedPacketEndIndex + 1)));
					input = input.Substring(nestedPacketEndIndex + 1, input.Length - nestedPacketEndIndex - 1);
				}
				else
				{
					return packet;
				}
			}

			return packet;
		}

		private static bool? ComparePacketOrder(Packet left, Packet right)
		{
			var largest = left.NextKey > right.NextKey ? left : right;
			for (var i = 0; i < largest.NextKey; i++)
			{
				if (left.NextKey <= i) { return true; }
				else if (right.NextKey <= i) { return false; }
				else if (left.IsInteger(i) && right.IsInteger(i))
				{
					if (left.Integers[i] < right.Integers[i]) { return true; }
					else if (left.Integers[i] > right.Integers[i]) { return false; }
				}
				else if (left.IsPacket(i) && right.IsPacket(i))
				{
					var result = ComparePacketOrder(left.Packets[i], right.Packets[i]);
					if (result.HasValue) { return result.Value; }
				}
				else
				{
					var leftPacket = left.IsPacket(i) ? left.Packets[i] : new Packet { Integers = new Dictionary<int, int>{ { 0, left.Integers[i] } }};
					var rightPacket = right.IsPacket(i) ? right.Packets[i] : new Packet { Integers = new Dictionary<int, int>{ { 0, right.Integers[i] } }};

					var result = ComparePacketOrder(leftPacket, rightPacket);
					if (result.HasValue) { return result.Value; }
				}
			}

			return null;
		}

		private class Packet
		{
			internal int Id { get; set; }

			internal Dictionary<int, int> Integers { get; set; } = new ();
			internal Dictionary<int, Packet> Packets { get; set; } = new();

			internal int NextKey => Math.Max(Integers.Keys.LastOr(-1), Packets.Keys.LastOr(-1)) + 1;

			internal bool IsInteger(int index)
			{
				return this.Integers.ContainsKey(index);
			}

			internal bool IsPacket(int index)
			{
				return this.Packets.ContainsKey(index);
			}

			public override string ToString()
			{
				var stringData = new List<string>();
				for (var i = 0; i < this.NextKey; i++)
				{
					if (Integers.TryGetValue(i, out var integer))
					{
						stringData.Add(integer.ToString());
					}
					else if (Packets.TryGetValue(i, out var packet))
					{
						stringData.Add(packet.ToString());
					}
				}

				return $"[{stringData.Join(",")}]";
			}
		}

		private const string TestInput = @"[1,1,3,1,1]
[1,1,5,1,1]

[[1],[2,3,4]]
[[1],4]

[9]
[[8,7,6]]

[[4,4],4,4]
[[4,4],4,4,4]

[7,7,7,7]
[7,7,7]

[]
[3]

[[[]]]
[[]]

[1,[2,[3,[4,[5,6,7]]]],8,9]
[1,[2,[3,[4,[5,6,0]]]],8,9]";
	}

	public static class Day13Extensions
	{
		public static T LastOr<T>(this IEnumerable<T> input, T defaultValue)
		{
			return input.Any() ? input.Last() : defaultValue;
		}
	}
}
