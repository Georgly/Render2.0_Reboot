using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    class Vertex
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vertex()
        {
            X = 0;
            Y = 0;
        }

        public Vertex(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vertex operator -(Vertex left, Vertex right)
        {
            return new Vertex(left.X - right.X, left.Y - right.Y);
        }

        public static Vertex operator +(Vertex left, Vertex right)
        {
            return new Vertex(left.X + right.X, left.Y + right.Y);
        }

        public static Vertex operator /(Vertex left, int n)
        {
            return new Vertex(left.X / n, left.Y / n);
        }

        public static Vertex operator /(Vertex left, double n)
        {
            return new Vertex(left.X / n, left.Y / n);
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }
    }
}
