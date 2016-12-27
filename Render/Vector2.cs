using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    class Vector2
    {
        public double X { get; set; }
        public double Y { get; set; }
        //double _finalX;
        //double _finalY;

        public Vector2()
        {
            X = 0;
            Y = 0;
            //FinalX = Convert.ToInt32(Constants.center + (x - z * Math.Cos(Constants.angl / 180 * Math.PI) / 2) * Constants.zoom);
            //FinalY = Convert.ToInt32(Constants.center - (y + z * Math.Sin(Constants.angl / 180 * Math.PI) / 2) * Constants.zoom);
        }

        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
            //FinalX = Convert.ToInt32(Constants.center + (x - z * Math.Cos(Constants.angl / 180 * Math.PI) / 2) * Constants.zoom);
            //FinalY = Convert.ToInt32(Constants.center - (y + z * Math.Sin(Constants.angl / 180 * Math.PI) / 2) * Constants.zoom);
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            Vector2 result = new Vector2();
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            return result;
        }

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            Vector2 result = new Vector2();
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            return result;
        }

        public static Vector2 operator /(Vector2 left, int n)
        {
            Vector2 result = new Vector2();
            result.X = left.X / n;
            result.Y = left.Y / n;
            return result;
        }

        public static Vector2 operator /(Vector2 left, double n)
        {
            Vector2 result = new Vector2();
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
