namespace AdventOfCode.Logic.Modules
{
    using System;
    using static AdventOfCode.Logic.Modules.Grid;

    internal abstract class Grid
    {
        internal static TGrid CreateGrid<TGrid, TNode>(int width, int height, Action<int, int, TNode> expandedConstruction = null)
			where TGrid : Grid<TNode>, new()
            where TNode : Node, new()
        {
            var nodes = new List<TNode>();

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var node = new TNode
                    {
                        Id = nodes.Count,
                        PosX = x,
                        PosY = y
                    };

                    expandedConstruction?.Invoke(x, y, node);

                    nodes.Add(node);
                }
            }

            return new TGrid
            {
                Width = width,
                Height = height,
                Nodes = nodes.ToDictionary(x => x.Id, x => x)
            };
        }

        internal static Grid<T> CreateGrid<T>(int width, int height, Action<int, int, T> expandedConstruction = null) where T : Node, new()
	    {
		    var nodes = new List<T>();

		    for (var y = 0; y < height; y++)
		    {
			    for (var x = 0; x < width; x++)
			    {
				    var node = new T
				    {
					    Id = nodes.Count,
					    PosX = x,
					    PosY = y
				    };

					expandedConstruction?.Invoke(x, y, node);

				    nodes.Add(node);
			    }
		    }

		    return new Grid<T>
		    {
			    Width = width,
			    Height = height,
			    Nodes = nodes.ToDictionary(x => x.Id, x => x)
		    };
	    }

	    internal class Node
	    {
		    internal int Id { get; init; }
		    internal int PosX { get; init; }
		    internal int PosY { get; init; }
		    internal List<int> Connections { get; set; } = new();
	    }

        internal class ConnectionOptions
        {
            internal bool IncludeDiagonal { get; set; } = false;
            internal int StepSize { get; set; } = 1;
        }
    }

    internal class Grid<T> : Grid where T : Node, new()
    {
        internal int Width { get; init; }

        internal int Height { get; init; }

        internal Dictionary<int, T> Nodes { get; init; }

        internal Func<int, int, int> NodeWeight = (_, _) => 1;
    }

    internal static class GridExtensions
    {
        internal static Grid<T> AddAllConnections<T>(this Grid<T> grid, Func<T, T, bool> conditionPredicate = null) where T : Node, new() => grid.AddAllConnections(null, conditionPredicate);

        internal static Grid<T> AddAllConnections<T>(this Grid<T> grid, Action<ConnectionOptions> optionBuilder, Func<T, T, bool> conditionPredicate = null) where T : Node, new()
        {
            var options = new ConnectionOptions();
            optionBuilder?.Invoke(options);

            foreach (var key in grid.Nodes.Keys)
            {
                var node = grid.Nodes[key];

                var hasLeft = node.Id % grid.Width >= options.StepSize;
                var hasUp = node.Id >= (grid.Width * options.StepSize);
                var hasRight = (node.Id + options.StepSize) % grid.Width >= options.StepSize;
                var hasDown = (node.Id + (grid.Width * options.StepSize)) < grid.Width * grid.Height;

                node.Connections.Add(hasLeft ? node.Id - options.StepSize : -1);
                node.Connections.Add(hasUp ? node.Id - (grid.Width * options.StepSize) : -1);
                node.Connections.Add(hasRight ? node.Id + options.StepSize : -1);
                node.Connections.Add(hasDown ? node.Id + (grid.Width * options.StepSize) : -1);

                if (options.IncludeDiagonal)
                {
                    node.Connections.Add(hasLeft && hasUp ? (node.Id - (grid.Width * options.StepSize)) - options.StepSize : -1);
                    node.Connections.Add(hasUp && hasRight ? (node.Id - (grid.Width * options.StepSize)) + options.StepSize : -1);
                    node.Connections.Add(hasLeft && hasDown ? (node.Id + (grid.Width * options.StepSize)) - options.StepSize : -1);
                    node.Connections.Add(hasRight && hasDown ? (node.Id + (grid.Width * options.StepSize)) + options.StepSize : -1);
                }

                node.Connections = node.Connections.Where(x =>
                    x != -1 && (conditionPredicate == null || conditionPredicate(node, grid.Nodes[x]))).ToList();
            }

            return grid;
        }

        internal static Grid<T> SetNodeDistanceMethod<T>(this Grid<T> grid, Func<int, int, int> func) where T : Node, new()
        {
            grid.NodeWeight = func;

            return grid;
        }

        internal static List<(List<int> route, int length)> CalculateAllPossiblePaths<T>(this Grid<T> grid, int startIndex, int endIndex)
            where T : Node, new()
        {
            var possibleRoutes = new List<(List<int> route, int length)>();
            var checkPaths = new List<List<int>> { new List<int> { startIndex } };

            while (true)
            {
                var newPaths = new List<List<int>>();
                foreach (var path in checkPaths.OrderBy(grid.PathWeight))
                {
                    var leaf = path.Last();
                    var possibilities = grid.Nodes[leaf].Connections
                        .Where(x => !path.Contains(x))
                        .ToList();

                    var leafPaths = possibilities.Select(x => new List<List<int>> { path, new() { x } }.SelectMany(y => y).ToList()).ToList();
                    var completePaths = leafPaths.Where(x => x.Last() == endIndex);

                    if (completePaths.Any())
                    {
                        possibleRoutes.AddRange(completePaths.Select(x => (x, grid.PathWeight(x))));
                        leafPaths.RemoveAll(x => completePaths.Contains(x));
                    }

                    newPaths.AddRange(leafPaths);
                }

                checkPaths = newPaths;
                if (!checkPaths.Any())
                {
                    return possibleRoutes;
                }
            }
        }

        internal static List<T> GetConnectedNodeGroup<T>(this Grid<T> grid, T root) where T : Node, new()
        {
            var nodeDict = new Dictionary<int, T>
            {
                { root.Id, root }
            };

            var leafs = new List<T> { root };

            while (true)
            {
                if (!leafs.Any())
                {
                    break;
                }

                var newLeafs = new List<T>();

                leafs.ForEach(leaf =>
                {
                    leaf.Connections.ForEach(connection =>
                    {
                        if (nodeDict.TryAdd(connection, grid.Nodes[connection]))
                        {
                            newLeafs.Add(grid.Nodes[connection]);
                        }
                    });
                });

                leafs = newLeafs;
            }

            return nodeDict.Values.ToList();
        }


        internal static (List<int> route, int length) CalculateShortestPath<T>(this Grid<T> grid, int startIndex, int endIndex) where T : Node, new()
        {
            var nodesVisited = new SafeDictionary<int, bool>
            {
                [startIndex] = true
            };

            var checkPaths = new List<List<int>> { new List<int> { startIndex } };

            while (true)
            {
                var newPaths = new List<List<int>>();
                foreach (var path in checkPaths.OrderBy(grid.PathWeight))
                {
                    var leaf = path.Last();
                    var possibilities = grid.Nodes[leaf].Connections
                        .Where(x => !path.Contains(x))
                        .Where(x => !nodesVisited[x])
                        .ToList();

                    var leafPaths = possibilities.Select(x => new List<List<int>> { path, new() { x } }.SelectMany(y => y).ToList()).ToList();
                    var completePath = leafPaths.SingleOrDefault(x => x.Last() == endIndex);

                    if (completePath != null)
                    {
                        return (completePath, grid.PathWeight(completePath));
                    }

                    possibilities.ForEach(x => nodesVisited[x] = true);
                    newPaths.AddRange(leafPaths);
                }

                checkPaths = newPaths;
                if (!checkPaths.Any())
                {
                    return (null, -1);
                }
            }
        }

        internal static int PathWeight<T>(this Grid<T> grid, List<int> nodes) where T : Node, new()
        {
            var total = 0;

            for (var i = 0; i < nodes.Count - 1; i++)
            {
                total += grid.NodeWeight(nodes[i], nodes[i + 1]);
            }

            return total;
        }
    }
}
