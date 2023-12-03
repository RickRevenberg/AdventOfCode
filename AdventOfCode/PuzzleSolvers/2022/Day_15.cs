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

    public class Day_15 : DayBase2022
	{
		public override int Day => 15;
		private List<Sensor> sensors;

		private const int GridSize = 4000000;

		[SetUp]
		public async Task SetUp()
		{
			var input = await this.SplitInput();
			var numericRegex = new Regex("-?[0-9]+");

			this.sensors = input.Select(line =>
			{
				var matches = numericRegex.Matches(line).Select(x => x.Value.ToInt()).ToList();
				
				return new Sensor
				{
					PosX = matches[0], PosY = matches[1],
					ClosestBeaconDistance = Math.Abs(matches[2] - matches[0]) + Math.Abs(matches[3] - matches[1])
				};
			}).ToList();
		}

		[Test]
		public void PartOne()
		{
			DetermineBlockedSpotCount(GridSize / 2).size.Pass();
		}

		[Test]
		public void PartTwo()
		{
			for (var i = 0; i <= GridSize; i++)
			{
				var (size, ranges) = DetermineBlockedSpotCount(i, true);

				if (size < GridSize)
				{
					$"X: {ranges[0].endX + 1}, Y: {i}. Frequency: {((long)ranges[0].endX + 1) * 4000000 + i}".Pass();
				}
			}
		}

		private (int size, List<(int startX, int endX)> blocked) DetermineBlockedSpotCount(int y, bool enforceBoundaries = false)
		{
			var blockedRanges = new List<(int startX, int endX)>();
			foreach (var sensor in this.sensors)
			{
				if (sensor.PosY - sensor.ClosestBeaconDistance > y || sensor.PosY + sensor.ClosestBeaconDistance < y)
				{
					continue;
				}

				var distanceDifference = sensor.ClosestBeaconDistance - (Math.Abs(sensor.PosY - y));
				blockedRanges.Add(
					enforceBoundaries 
						? (Math.Max(0, sensor.PosX - distanceDifference), Math.Min(GridSize, sensor.PosX + distanceDifference))
						: (sensor.PosX - distanceDifference, sensor.PosX + distanceDifference));
			}

			blockedRanges = blockedRanges.OrderBy(x => x.startX).ToList();
			blockedRanges = blockedRanges.Where(range => blockedRanges.Count(range2 => range.startX >= range2.startX && range.endX <= range2.endX) < 2).ToList();

			var tracker = 1;
			while (true)
			{
				if (tracker == blockedRanges.Count)
				{
					break;
				}

				for (var i = tracker; i < blockedRanges.Count; i++)
				{
					if (blockedRanges[i].startX <= blockedRanges[i - 1].endX)
					{
						blockedRanges[i - 1] = (blockedRanges[i - 1].startX, blockedRanges[i].endX);
						blockedRanges.Remove(blockedRanges[i]);
						break;
					}

					tracker++;
				}
			}

			return (blockedRanges.Select(x => Math.Abs(x.endX - x.startX)).Sum(), blockedRanges);
		}

		private class Sensor
		{
			internal int PosX { get; init; }
			internal int PosY { get; init; }
			internal int ClosestBeaconDistance { get; init; }

			public override string ToString()
			{
				return $"{PosX} - {PosY}, {ClosestBeaconDistance}";
			}
		}
	}
}
