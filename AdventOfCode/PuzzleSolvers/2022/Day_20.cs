namespace AdventOfCode.PuzzleSolvers._2022
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AdventOfCode.Logic.Extensions;
    using Logic;
    using NUnit.Framework;

    public class Day_20 : DayBase2022
	{
		public override int Day => 20;

		private int collectionSize;
		private Dictionary<int, LinkedListNode<long>> nodeDict;

		[SetUp]
		public async Task SetUp()
		{
			var nodes = (await this.SplitInput())
				.Select(x => x.ToInt())
				.Select((number, index) => new LinkedListNode<long> { Value = number, TrackingId = index }).ToList();

			this.nodeDict = nodes.ToDictionary(x => x.TrackingId, x => x);

			for (var i = 1; i < nodes.Count; i++)
			{
				nodes[i - 1].Next = nodes[i];
				nodes[i].Previous = nodes[i - 1];
			}

			nodes.First().Previous = nodes.Last();
			nodes.Last().Next = nodes.First();

			collectionSize = nodes.Count;
		}

		[Test]
		public void PartOne()
		{
			this.MixCollection(1);
			this.DetermineAnswer();
		}

		[Test]
		public void PartTwo()
		{
			const long DecryptionKey = 811589153;
			nodeDict.Values.ToList().ForEach(node => node.Value *= DecryptionKey);

			this.MixCollection(10);
			this.DetermineAnswer();
		}

		private void MixCollection(int times)
		{
			for (var j = 0; j < times; j++)
			{
				for (var i = 0; i < this.collectionSize; i++)
				{
					var trackingNode = this.nodeDict[i];

					var usednumber = trackingNode.Value;
					var positive = usednumber >= 0;

					var steps = Math.Abs(usednumber);

					steps %= (collectionSize - 1);
					steps = positive ? steps : collectionSize - steps - 1;

					if (steps == 0)
					{
						continue;
					}

					var indexNode = trackingNode;
					for (var k = 0; k < steps; k++)
					{
						indexNode = indexNode.Next;
					}

					trackingNode.Previous.Next = trackingNode.Next;
					trackingNode.Next.Previous = trackingNode.Previous;

					indexNode.Next.Previous = trackingNode;
					trackingNode.Next = indexNode.Next;
					indexNode.Next = trackingNode;
					trackingNode.Previous = indexNode;
				}
			}
		}

		private void DetermineAnswer()
		{
			var trackingNode = nodeDict[0];
			while (true)
			{
				if (trackingNode.Value == 0)
				{
					break;
				}

				trackingNode = trackingNode.Next;
			}

			var foundNumbers = new List<long>();
			var zeroIndex = trackingNode.TrackingId;

			for (var i = 1; i <= 3; i++)
			{
				trackingNode = nodeDict[zeroIndex];
				var targetIndex = (i * 1000) % this.collectionSize;

				for (var j = 0; j < targetIndex; j++)
				{
					trackingNode = trackingNode.Next;
				}

				foundNumbers.Add(trackingNode.Value);
			}

			foundNumbers.Sum().Pass();
		}

		
		private class LinkedListNode<T>
		{
			internal int TrackingId { get; init; }

			internal LinkedListNode<T> Previous { get; set; }
			internal LinkedListNode<T> Next { get; set; }

			internal T Value { get; set; }
		}
	}
}
