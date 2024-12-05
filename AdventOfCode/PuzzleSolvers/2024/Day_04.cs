namespace AdventOfCode.PuzzleSolvers._2024
{
    using AdventOfCode.Logic.Extensions;
    using Logic.Modules;
    using System;

    [TestFixture]
    public class Day_04 : DayBase2024
    {
        public override int Day => 4;

        private PathFinder.Grid<WordSearchNode> grid;

        [SetUp]
        public async Task SetUp()
        {
            var nodes = (await this.SplitInput()).Select(x => x.ToList().ToList()).ToList();

            this.grid = PathFinder.CreateGrid<WordSearchNode>(nodes[0].Count, nodes.Count, (x, y, node) =>
            {
                node.Letter = nodes[y][x].ToString();
            });

            this.grid.AddAllConnections(options => options.IncludeDiagonal = true);
        }

        [Test]
        public void PartOne()
        {
            var wordCount = FindWords("XMAS").Count;
            wordCount.Pass();
        }

        [Test]
        public void PartTwo()
        {
            var foundWords = FindWords("MAS").Where(x => x[0].PosX != x[2].PosX && x[0].PosY != x[2].PosY);
            var wordPairs = foundWords.Where(x => foundWords.Count(y => x[1] == y[1]) > 1).ToList();

            var foundPairs = wordPairs.Count / 2;
            foundPairs.Pass();
        }

        private List<List<WordSearchNode>> FindWords(string word)
        {
            var startingLetter = word.ToList()[0].ToString();

            var startingNodes = this.grid.Nodes.Values.Where(x => x.Letter == startingLetter).ToList();
            var directions = Enum.GetValues<Direction>();

            var foundWords = new List<List<WordSearchNode>>();

            foreach (var node in startingNodes)
            {
                foreach (var direction in directions)
                {
                    var diff = GetDirectionDiff(direction);
                    var furthestNodeDiff = (diff.x * (word.Length - 1), diff.y * (word.Length - 1));

                    var furtherstNodeLoc = (node.PosX + furthestNodeDiff.Item1, node.PosY + furthestNodeDiff.Item2);

                    if (furtherstNodeLoc.Item1 < 0 || furtherstNodeLoc.Item1 >= this.grid.Width ||
                        furtherstNodeLoc.Item2 < 0 || furtherstNodeLoc.Item2 >= this.grid.Height)
                    {
                        continue;
                    }

                    List<WordSearchNode> nodes = [node];
                    for (var i = 0; i < (word.Length - 1); i++)
                    {
                        nodes.Add(this.grid.Nodes[nodes.Last().Connections.Single(
                            connId => this.grid.Nodes[connId].PosX == nodes.Last().PosX + diff.x &&
                                      this.grid.Nodes[connId].PosY == nodes.Last().PosY + diff.y)]);
                    }

                    var foundWord = nodes.Select(x => x.Letter).Join();
                    if (foundWord == word)
                    {
                        foundWords.Add(nodes);
                    }
                }
            }

            
            return foundWords;

            (int x, int y) GetDirectionDiff(Direction direction)
            {
                return direction switch
                {
                    Direction.Up => (0, 1),
                    Direction.UpRight => (1, 1),
                    Direction.Right => (1, 0),
                    Direction.DownRight => (1, -1),
                    Direction.Down => (0, -1),
                    Direction.DownLeft => (-1, -1),
                    Direction.Left => (-1, 0),
                    Direction.UpLeft => (-1, 1),
                    _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
                };
            }
        }

        private class WordSearchNode : PathFinder.Node
        {
            internal string Letter { get; set; }
        }

        private enum Direction
        {
            Up,
            UpRight,
            Right,
            DownRight,
            Down,
            DownLeft,
            Left,
            UpLeft
        }
    }
}
