namespace AdventOfCode.PuzzleSolvers._2022
{
    using Logic.Extensions;
    using Logic.Modules;
    using System;

    public class Day_24 : DayBase2022
    {
        public override int Day => 24;

        private string TestInput => @"#.######
#>>.<^<#
#.<..<<#
#>v.><>#
#<^v^^>#
######.#".Replace("\r", "");

        private Dictionary<int, Node> grid;

        [SetUp]
        public async Task SetUp()
        {
            this.grid = new();

            var input = await this.GetInput();

            var (lengthX, lengthY) = CreateGrid(input, out var blizzards);
            
            SetBlockedTurnCounts(lengthX, lengthY, blizzards);
            SetNeighbourLinks(lengthX);

            this.grid.Add(-1, new Node(-1, 0, -1)
            {
                Neighbours = [0]
            });

            this.grid.Add(this.grid.Keys.Max() + 1, new Node(this.grid.Keys.Max() + 1, lengthX - 1, lengthY)
            {
                Neighbours = [this.grid.Keys.Max()]
            });

            this.grid[0].Neighbours.Add(-1);
            this.grid[this.grid.Keys.Max() - 1].Neighbours.Add(this.grid.Keys.Max());
        }

        private (int lengthX, int lengthY) CreateGrid(string input, out List<(int posX, int posY, BlizzardDirection direction)> blizzards)
        {
            var lines = input.Split("\n");
            var lengthX = lines[0].Length - 2;
            var lengthY = lines.Length - 2;

            blizzards = [];

            for (var y = 0; y < lengthY; y++)
            {
                for (var x = 0; x < lengthX; x++)
                {
                    grid.Add(grid.Keys.Count, new Node(grid.Keys.Count, x, y));

                    var sign = lines[y + 1][x + 1];
                    if (sign == '.')
                    {
                        continue;
                    }

                    var direction = sign switch
                    {
                        '<' => BlizzardDirection.Left,
                        '^' => BlizzardDirection.Up,
                        '>' => BlizzardDirection.Right,
                        'v' => BlizzardDirection.Down,
                        _ => throw new Exception()
                    };

                    blizzards.Add((x, y, direction));
                }
            }

            return (lengthX, lengthY);
        }

        private void SetBlockedTurnCounts(int lengthX, int lengthY, List<(int posX, int posY, BlizzardDirection direction)> blizzards)
        {
            var timeToReset = lengthX * lengthY;
            var xTimeCounts = new List<int>();
            var yTimeCounts = new List<int>();

            for (var x = 0; x < timeToReset; x += lengthX)
            {
                xTimeCounts.Add(x);
            }

            for (var y = 0; y < timeToReset; y += lengthY)
            {
                yTimeCounts.Add(y);
            }

            foreach (var blizzard in blizzards)
            {
                var nodes = blizzard.direction.In(BlizzardDirection.Left, BlizzardDirection.Right)
                    ? grid.Values.Where(x => x.PosY == blizzard.posY).ToList()
                    : grid.Values.Where(x => x.PosX == blizzard.posX).ToList();

                var startPosition = blizzard.direction.In(BlizzardDirection.Left, BlizzardDirection.Right) ? blizzard.posX : blizzard.posY;
                if (blizzard.direction.In(BlizzardDirection.Left, BlizzardDirection.Up))
                {
                    nodes.Reverse();
                    startPosition = (nodes.Count - 1) - startPosition;
                }

                for (var i = 0; i < startPosition; i++)
                {
                    nodes.Add(nodes[i]);
                }

                nodes = nodes.Skip(startPosition).ToList();
                var usedCounts = blizzard.direction.In(BlizzardDirection.Left, BlizzardDirection.Right) ? xTimeCounts : yTimeCounts;

                for (var i = 0; i < nodes.Count(); i++)
                {
                    usedCounts.Select(x => x + i).ToList().ForEach(x => nodes[i].BlockedTurnIds.TryAdd(x, false));
                }
            }
        }

        private void SetNeighbourLinks(int lengthX)
        {
            foreach (var node in grid.Values)
            {
                node.Neighbours.Add(node.PosX > 0 ? node.Id - 1 : -1);
                node.Neighbours.Add((node.PosX + 1) % lengthX > 0 ? node.Id + 1 : -1);
                node.Neighbours.Add(node.PosY > 0 ? node.Id - lengthX : -1);
                node.Neighbours.Add(node.Id + lengthX < grid.Keys.Count ? node.Id + lengthX : -1);

                node.Neighbours = node.Neighbours.Where(x => x != -1).ToList();
            }
        }

        [Test]
        public void PartOne()
        {
            var foundPath = GetBestPath(-1, this.grid.Count - 2);
            (foundPath.Count - 1).Pass();
        }

        [Test]
        public void PartTwo()
        {
            var firstPart = GetBestPath(-1, this.grid.Count - 2).Skip(1);
            var secondPart = GetBestPath(this.grid.Count - 2, -1, firstPart.Count()).Skip(1);
            var thirdPart = GetBestPath(-1, this.grid.Count - 2, firstPart.Count() + secondPart.Count()).Skip(1);

            var answer = (firstPart.Count() + secondPart.Count() + thirdPart.Count());

            answer.Pass();
        }

        private List<int> GetBestPath(int startId, int goalId, int turnOffset = 0)
        {
            var nodesVisited = new SafeDictionary<int, List<int>>(defaultValue: _ => [])
            {
                [startId] = [0]
            };

            var paths = new List<List<int>> { new() { startId } };
            var endNode = this.grid[goalId];

            var timeTickover = this.grid.Values.Select(x => x.PosX + 1).Max() * this.grid.Values.Select(x => x.PosY).Max();

            while (true)
            {
                var nextBestPath = paths
                    .OrderBy(x => Math.Abs(endNode.PosX - this.grid[x.Last()].PosX) + Math.Abs(endNode.PosY - this.grid[x.Last()].PosY) + x.Count)
                    .First();

                if (nextBestPath.Last() == goalId)
                {
                    break;
                }

                var possibilities = this.grid[nextBestPath.Last()].Neighbours
                    .Where(id => !nodesVisited[id].Contains(nextBestPath.Count))
                    .Where(id => !this.grid[id].IsBlocked((nextBestPath.Count + turnOffset) % timeTickover))
                    .ToList();

                possibilities.ForEach(id =>
                {
                    nodesVisited[id].Add(nextBestPath.Count);
                    var newPath = nextBestPath.Clone();
                    newPath.Add(id);

                    paths.Add(newPath);
                });

                if (!this.grid[nextBestPath.Last()].IsBlocked((nextBestPath.Count + turnOffset) % timeTickover))
                {
                    var newPath = nextBestPath.Clone();
                    newPath.Add(nextBestPath.Last());

                    paths.Add(newPath);
                }

                paths.Remove(nextBestPath);
            }

            return paths.Where(x => x.Last() == goalId).OrderBy(x => x.Count).First();
        }

        private enum BlizzardDirection
        {
            Up, Right, Down, Left
        }

        private class Node
        {
            internal readonly int Id, PosX, PosY;

            internal List<int> Neighbours { get; set; } = [];

            internal Dictionary<int, bool> BlockedTurnIds { get; set; } = [];

            internal Node(int id, int posX, int posY)
            {
                this.Id = id;
                this.PosX = posX;
                this.PosY = posY;
            }

            internal bool IsBlocked(int turnId)
            {
                return this.BlockedTurnIds.ContainsKey(turnId);
            }

            public override string ToString()
            {
                return $"{this.Id}: {this.PosX}-{this.PosY}";
            }
        }
    }
}
