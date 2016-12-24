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
        //double _finalX;
        //double _finalY;

        public Vertex()
        {
            X = 0;
            Y = 0;
            //FinalX = Convert.ToInt32(Constants.center + (x - z * Math.Cos(Constants.angl / 180 * Math.PI) / 2) * Constants.zoom);
            //FinalY = Convert.ToInt32(Constants.center - (y + z * Math.Sin(Constants.angl / 180 * Math.PI) / 2) * Constants.zoom);
        }

        public Vertex(double x, double y)
        {
            X = x;
            Y = y;
            //FinalX = Convert.ToInt32(Constants.center + (x - z * Math.Cos(Constants.angl / 180 * Math.PI) / 2) * Constants.zoom);
            //FinalY = Convert.ToInt32(Constants.center - (y + z * Math.Sin(Constants.angl / 180 * Math.PI) / 2) * Constants.zoom);
        }

        public static Vertex operator -(Vertex left, Vertex right)
        {
            Vertex result = new Vertex();
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            return result;
        }

        public static Vertex operator +(Vertex left, Vertex right)
        {
            Vertex result = new Vertex();
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            return result;
        }

        public static Vertex operator /(Vertex left, int n)
        {
            Vertex result = new Vertex();
            result.X = left.X / n;
            result.Y = left.Y / n;
            return result;
        }

        public static Vertex operator /(Vertex left, double n)
        {
            Vertex result = new Vertex();
            result.X = left.X / n;
            result.Y = left.Y / n;
            return result;
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        //public double FinalX
        //{
        //    get { return _finalX; }
        //    set { _finalX = value; }
        //}

        //public double FinalY
        //{
        //    get { return _finalY; }
        //    set { _finalY = value; }
        //}

    }
}
