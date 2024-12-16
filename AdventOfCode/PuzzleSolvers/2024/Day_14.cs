namespace AdventOfCode.PuzzleSolvers._2024
{
    using Logic.Extensions;
    using Logic.Modules;
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    public class Day_14 : DayBase2024
    {
        public override int Day => 14;

        private Grid<Grid.Node> grid;
        private List<Robot> robots;

        [SetUp]
        public async Task SetUp()
        {
            var input = await this.SplitInput();

            this.grid = Grid.CreateGrid<Grid.Node>(101, 103);
            this.robots = input.Select(x =>
            {
                var digits = Regex.Matches(x, "-?[0-9]+").Select(x => int.Parse(x.Value)).ToList();
                return new Robot { LocationNodeId = CoordinatesToId((digits[0], digits[1])), Speed = (digits[2], digits[3]) };
            }).ToList();

            this.grid.AddAllConnections();
        }

        [Test]
        public void PartOne()
        {
            for (var i = 0; i < 100; i++)
            {
                MoveRobotsOneTick();
            }

            var quadrants = new List<int> { 0, 0, 0, 0 };

            var splitLineX = this.grid.Width / 2;
            var splitLineY = this.grid.Height / 2;

            var splitBots = new List<Robot>();

            foreach (var robot in this.robots)
            {
                var node = this.grid.Nodes[robot.LocationNodeId];
                if (node.PosX == splitLineX || node.PosY == splitLineY)
                {
                    splitBots.Add(robot);
                    continue;
                }

                var quadrantId = (node.PosX < splitLineX ? 0 : 1) + (node.PosY < splitLineY ? 0 : 2);
                quadrants[quadrantId]++;
            }

            quadrants.Product().Pass();
        }

        [Test] // Answer = 7344
        public void PartTwo()
        {
            /*var tracker = 1;
            while (true)
            {
                MoveRobotsOneTick();

                var grid = DrawGrid();
                var lines = grid.Split().Where(x => !string.IsNullOrEmpty(x) && x.Count(y => y == '.') < 80).ToList();

                if (lines.Any())
                {
                    var what = 0;
                }

                tracker++;
            }*/

            const int FoundAnswer = 7344;
            for (var i = 0; i < FoundAnswer; i++)
            {
                MoveRobotsOneTick();
            }

            Console.WriteLine(FoundAnswer);
            Console.WriteLine(DrawGrid());
        }

        private int CoordinatesToId((int x, int y) loc)
        {
            return (loc.y * this.grid.Width) + loc.x;
        }

        private void MoveRobotsOneTick()
        {
            foreach (var robot in this.robots)
            {
                var currentNode = this.grid.Nodes[robot.LocationNodeId];
                (int x, int y) newPosition = (currentNode.PosX + robot.Speed.X, currentNode.PosY + robot.Speed.Y);

                newPosition.x = (newPosition.x + this.grid.Width) % this.grid.Width;
                newPosition.y = (newPosition.y + this.grid.Height) % this.grid.Height;

                robot.LocationNodeId = CoordinatesToId(newPosition);
            }
        }

        private string DrawGrid()
        {
            var sb = new StringBuilder();
            var robotDict = new SafeDictionary<int, List<Robot>>((_) => []);

            this.robots.ForEach(robot => robotDict[robot.LocationNodeId].Add(robot));

            for (var y = 0; y < this.grid.Height; y++)
            {
                var line = "";
                for (var x = 0; x < this.grid.Width; x++)
                {
                    var robotCount = robotDict[CoordinatesToId((x, y))].Count;
                    line += robotCount > 0 ? robotCount.ToString() : ".";
                }

                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        private class Robot
        {
            internal int LocationNodeId { get; set; }

            internal (int X, int Y) Speed { get; set; }

            public override string ToString()
            {
                return $"[{LocationNodeId}]";
            }
        }
    }
}
