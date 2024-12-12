namespace AdventOfCode.Logic.Modules
{
    using System;

    internal class Vector2D
    {
        internal (int x, int y) From, To;

        internal (int deltaX, int deltaY) Difference => (To.x - From.x, To.y - From.y);

        internal Vector2D(int fromX, int fromY, int toX, int toY) : this((fromX, fromY), (toX, toY)) { }
        internal Vector2D((int x, int y) from, (int x, int y) to)
        {
            this.From = from;
            this.To = to;
        }

        internal (float dirX, float dirY) Direction()
        {
            var distanceX = (double) Math.Abs(To.x - From.x);
            var distanceY = (double) Math.Abs(To.y - From.y);

            var totalDistance = Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));

            var fractionX = (distanceX / totalDistance) / totalDistance;
            var fractionY = (distanceY / totalDistance) / totalDistance;

            return ((float) (distanceX * fractionX), (float) (distanceY * fractionY));
        }

        public override string ToString()
        {
            return $"({From.x}, {From.y}) => ({To.x}, {To.y}) - Diff: ({this.Difference.deltaX}, {this.Difference.deltaY})";
        }
    }
}
