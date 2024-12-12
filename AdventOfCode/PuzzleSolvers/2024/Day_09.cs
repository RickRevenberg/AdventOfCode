namespace AdventOfCode.PuzzleSolvers._2024
{
    using Logic.Extensions;
    using System;

    public class Day_09 : DayBase2024
    {
        public override int Day => 9;

        private Dictionary<int, int?> disk;

        [SetUp]
        public async Task SetUp()
        {
            disk = new();

            var input = (await this.SplitInput())[0].ToList().Select(x => (int.Parse(x.ToString()))).ToList();

            var fileId = 0;
            var tracker = 0;

            for (var i = 0; i < input.Count; i++)
            {
                for (var j = 0; j < input[i]; j++)
                {
                    disk.Add(tracker, i % 2 == 0 ? fileId : null);
                    tracker++;
                }

                fileId += i % 2 == 0 ? 1 : 0;
            }
        }

        [Test]
        public void PartOne()
        {
            var emptyIndexTracker = 0;
            var fileIndexTracker = this.disk.Keys.Count - 1;

            while (true)
            {
                if (emptyIndexTracker > fileIndexTracker)
                {
                    break;
                }

                var firstEmptyIndex = 0;
                for (; emptyIndexTracker < this.disk.Keys.Count; emptyIndexTracker++)
                {
                    if (this.disk[emptyIndexTracker] == null)
                    {
                        firstEmptyIndex = emptyIndexTracker;
                        break;
                    }
                }

                var lastFileIndex = 0;
                for (; fileIndexTracker >= 0; fileIndexTracker--)
                {
                    if (this.disk[fileIndexTracker] != null)
                    {
                        lastFileIndex = fileIndexTracker;
                        break;
                    }
                }

                disk[firstEmptyIndex] = disk[lastFileIndex];
                disk[lastFileIndex] = null;
            }

            this.disk.Values.Where(x => x != null).Select((x, i) => (long)x * i).Sum().Pass();
        }

        [Test]
        public void PartTwo()
        {
            var files = new List<File>();
            var diskList = this.disk.Values.ToList();

            for (var i = 0; i < this.disk.Count; i += 0)
            {
                var fileId = diskList[i];
                var nextBlockIndex = diskList.FindIndex(i, x => x != fileId);

                if (nextBlockIndex < 0)
                {
                    nextBlockIndex = this.disk.Count;
                }

                files.Add(new File { Id = fileId, StartPosition = i, Length = nextBlockIndex - i });
                i = nextBlockIndex;
            }

            files = files.Where(x => x.Id != null).ToList();
            var spaceCacheDict = new Dictionary<int, int>();

            var highestIndexReached = 0;

            foreach (var file in files.OrderByDescending(x => x.Id))
            {
                var startingIndex = highestIndexReached;

                var cachedLocation = spaceCacheDict.Where(x => x.Value >= file.Length).OrderBy(x => x.Key).FirstOrDefault();
                if (cachedLocation.Value != 0)
                {
                    startingIndex = cachedLocation.Key;
                    spaceCacheDict.Add(startingIndex + file.Length, spaceCacheDict[startingIndex] - file.Length);
                    spaceCacheDict.Remove(startingIndex);
                }

                for (var i = startingIndex; i < file.StartPosition; i++)
                {
                    if (this.disk[i] != null)
                    {
                        continue;
                    }

                    var nextBlockIndex = this.disk.First(x => x.Key > i && x.Value != null).Key;
                    var emptySpaceLength = nextBlockIndex - i;

                    if (emptySpaceLength >= file.Length)
                    {
                        for (var j = 0; j < file.Length; j++)
                        {
                            this.disk[i + j] = this.disk[file.StartPosition + j];
                            this.disk[file.StartPosition + j] = null;
                        }

                        highestIndexReached = Math.Max(highestIndexReached, i);

                        break;
                    }

                    spaceCacheDict.Add(i, emptySpaceLength);
                    i = nextBlockIndex - 1;
                }
            }

            this.disk.Values.Select((x, i) => (long)(x ?? 0) * i).Sum().Pass();
        }

        private class File
        {
            internal int? Id { get; set; }
            internal int StartPosition { get; set; }
            internal int Length { get; set; }

            public override string ToString()
            {
                return string.Concat(Enumerable.Repeat(this.Id?.ToString() ?? ".", this.Length));
            }
        }
    }
}
