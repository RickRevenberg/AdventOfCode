namespace AdventOfCode.Logic.Modules
{
    internal class GraphNode<T> where T : GraphNode<T>
    {
        internal int Id { get; init; }

        internal T Parent { get; set; }
        internal List<T> Children { get; set; } = new();

        internal bool IsRoot => this.Parent == null;

        internal int Depth => this.Parent == null ? 0 : 1 + this.Parent.Depth;
    }
}
