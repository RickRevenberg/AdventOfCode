namespace AdventOfCode.Logic.Modules
{
    using System;

    internal class Vector2D
    {
        internal (long x, long y) From, To;
        internal (long deltaX, long deltaY) Difference => (To.x - From.x, To.y - From.y);

        internal Vector2D(long fromX, long fromY, long toX, long toY) : this((fromX, fromY), (toX, toY)) { }
        internal Vector2D((long x, long y) from, (long x, long y) to)
        {
            this.From = from;
            this.To = to;
        }

        internal (double dirX, double dirY) Direction()
        {
            var distanceX = (double) Math.Abs(To.x - From.x);
            var distanceY = (double) Math.Abs(To.y - From.y);

            var totalDistance = Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));

            var fractionX = (distanceX / totalDistance) / totalDistance;
            var fractionY = (distanceY / totalDistance) / totalDistance;

            return ((double) (distanceX * fractionX), (double) (distanceY * fractionY));
        }

        internal double Angle()
        {
            var diff = this.Difference;

            var lengthX = (double)Math.Abs(diff.deltaX);
            var lengthY = (double)Math.Abs(diff.deltaY);

            var radians = Math.Atan(lengthY / lengthX);

            return radians * 180 / Math.PI;
        }

        internal double Length()
        {
            var diff = this.Difference;

            var distanceX = (double)Math.Abs(diff.deltaX);
            var distanceY = (double)Math.Abs(diff.deltaY);

            return Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));
        }

        public static Vector2D operator *(Vector2D vector, long multiplier)
        {
            return new Vector2D(vector.From, (vector.To.x * multiplier, vector.To.y * multiplier));
        }

        public static (long X, long Y) operator +((long X, long Y) point, Vector2D vector)
        {
            return (point.X + vector.Difference.deltaX, point.Y + vector.Difference.deltaY);
        }

        public static Vector2D operator +(Vector2D one, Vector2D two)
        {
            return new Vector2D(one.From.x + two.From.x, one.From.y + two.From.y, one.To.x + two.To.x,
                one.To.y + two.To.y);
        }

        public static bool operator ==(Vector2D one, Vector2D two)
        {
            return (one.From.x == two.From.x && one.From.y == two.From.y && one.To.x == two.To.x && one.To.y == two.To.y);
        }

        public static bool operator !=(Vector2D one, Vector2D two)
        {
            return !(one == two);
        }

        public override string ToString()
        {
            return $"({From.x}, {From.y}) => ({To.x}, {To.y}) - Diff: ({this.Difference.deltaX}, {this.Difference.deltaY})";
        }
    }
}
