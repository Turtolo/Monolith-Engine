using System;
using Microsoft.Xna.Framework;

namespace ConstructEngine.Area
{
    public class RectangleShape : IRegionShape2D, IEquatable<RectangleShape>
    {
        public Rectangle Rect;

        public RectangleShape(int x, int y, int width, int height)
        {
            Rect = new Rectangle(x, y, width, height);
        }

        public RectangleShape(Rectangle rect)
        {
            Rect = rect;
        }

        public int X { get => Rect.X; set => Rect.X = value; }
        public int Y { get => Rect.Y; set => Rect.Y = value; }
        public int Width { get => Rect.Width; set => Rect.Width = value; }
        public int Height { get => Rect.Height; set => Rect.Height = value; }

        public int Left => Rect.Left;
        public int Right => Rect.Right;
        public int Top => Rect.Top;
        public int Bottom => Rect.Bottom;

        public bool IsEmpty => Rect.IsEmpty;

        public Point Location
        {
            get => Rect.Location;
            set => Rect.Location = value;
        }

        public Point Center => Rect.Center;

        public Point Size
        {
            get => new Point(Rect.Width, Rect.Height);
            set
            {
                Rect.Width = value.X;
                Rect.Height = value.Y;
            }
        }

        public void Offset(int offsetX, int offsetY) => Rect.Offset(offsetX, offsetY);
        public void Offset(Point amount) => Rect.Offset(amount);
        public void Inflate(int horizontal, int vertical) => Rect.Inflate(horizontal, vertical);
        public bool Contains(Point p) => Rect.Contains(p);
        public bool Contains(int x, int y) => Rect.Contains(x, y);
        public bool Contains(RectangleShape r) => Rect.Contains(r.Rect);
        public bool Contains(Rectangle r) => Rect.Contains(r);

        public void Deconstruct(out int x, out int y, out int width, out int height)
        {
            x = X;
            y = Y;
            width = Width;
            height = Height;
        }

        public bool Intersects(RectangleShape other) => Rect.Intersects(other.Rect);
        public bool Intersects(Rectangle other) => Rect.Intersects(other);

        public bool Intersects(IRegionShape2D other) =>
            other switch
            {
                RectangleShape r => Rect.Intersects(r.Rect),
                CircleShape c    => CircleIntersectsRectangle(c, Rect),
                _                => false
            };

        private static bool CircleIntersectsRectangle(CircleShape c, Rectangle r)
        {
            int closestX = Math.Clamp(c.X, r.Left, r.Right);
            int closestY = Math.Clamp(c.Y, r.Top, r.Bottom);

            int dx = c.X - closestX;
            int dy = c.Y - closestY;

            return dx * dx + dy * dy <= c.Radius * c.Radius;
        }

        public bool Equals(RectangleShape other)
        {
            if (other is null) return false;
            return Rect.Equals(other.Rect);
        }

        public override bool Equals(object obj) =>
            obj is RectangleShape r && Equals(r);

        public override int GetHashCode() => Rect.GetHashCode();

        public static bool operator ==(RectangleShape lhs, RectangleShape rhs)
        {
            if (ReferenceEquals(lhs, rhs)) return true;
            if (lhs is null || rhs is null) return false;
            return lhs.Rect == rhs.Rect;
        }

        public static bool operator !=(RectangleShape lhs, RectangleShape rhs) =>
            !(lhs == rhs);

        
        public static explicit operator Rectangle(RectangleShape r) => r.Rect;

            public static explicit operator RectangleShape(Rectangle r) => new RectangleShape(r);
        }
}
