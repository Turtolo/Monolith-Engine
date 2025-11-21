using System;
using Microsoft.Xna.Framework;

namespace ConstructEngine.Area
{
    public class CircleShape2D : IRegionShape2D, IEquatable<CircleShape2D>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Radius { get; set; }

        public CircleShape2D(int x, int y, int radius)
        {
            X = x;
            Y = y;
            Radius = radius;
        }

        public CircleShape2D(Point location, int radius)
        {
            X = location.X;
            Y = location.Y;
            Radius = radius;
        }

        // --- IRegionShape2D Implementation --- //
        public Point Location
        {
            get => new Point(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public void Offset(int x, int y)
        {
            X += x;
            Y += y;
        }

        public Rectangle BoundingBox =>
            new Rectangle(X - Radius, Y - Radius, Radius * 2, Radius * 2);

        public bool Contains(Point p)
        {
            int dx = p.X - X;
            int dy = p.Y - Y;
            return dx * dx + dy * dy <= Radius * Radius;
        }

        public bool Contains(IRegionShape2D other)
        {
            return other switch
            {
                CircleShape2D c    => ContainsCircle(c),
                RectangleShape2D r => ContainsRect(r.Rect),
                _                  => false
            };
        }

        private bool ContainsCircle(CircleShape2D other)
        {
            float dist = Vector2.Distance(new Vector2(X, Y), new Vector2(other.X, other.Y));
            return dist + other.Radius <= Radius;
        }

        private bool ContainsRect(Rectangle r)
        {
            return Contains(r.Location) &&
                   Contains(new Point(r.Right, r.Top)) &&
                   Contains(new Point(r.Left, r.Bottom)) &&
                   Contains(new Point(r.Right, r.Bottom));
        }

        public bool Intersects(IRegionShape2D other)
        {
            return other switch
            {
                CircleShape2D c    => IntersectsCircle(c),
                RectangleShape2D r => IntersectsRectangle(r.Rect),
                _                  => false
            };
        }

        private bool IntersectsCircle(CircleShape2D other)
        {
            int sum = Radius + other.Radius;
            float distSq = Vector2.DistanceSquared(new Vector2(X, Y), new Vector2(other.X, other.Y));
            return distSq <= sum * sum;
        }

        private bool IntersectsRectangle(Rectangle r)
        {
            int closestX = Math.Clamp(X, r.Left, r.Right);
            int closestY = Math.Clamp(Y, r.Top, r.Bottom);
            int dx = X - closestX;
            int dy = Y - closestY;
            return dx * dx + dy * dy <= Radius * Radius;
        }

        public IRegionShape2D Clone() => new CircleShape2D(X, Y, Radius);

        // --- Equality --- //
        public bool Equals(CircleShape2D other) =>
            other != null && X == other.X && Y == other.Y && Radius == other.Radius;

        public override bool Equals(object obj) => obj is CircleShape2D c && Equals(c);

        public override int GetHashCode() => HashCode.Combine(X, Y, Radius);
    }
}
