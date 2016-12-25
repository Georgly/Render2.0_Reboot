using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    struct Vector
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector(float x, float y, float z)
        {
            Z = z;
            X = x;
            Y = y;
            //FinalX = Convert.ToInt32(Constants.center + (x - z * Math.Cos(Constants.angl / 180 * Math.PI) / 2) * Constants.zoom);
            //FinalY = Convert.ToInt32(Constants.center - (y + z * Math.Sin(Constants.angl / 180 * Math.PI) / 2) * Constants.zoom);
        }

        double Length()
        {
            return Math.Sqrt((X * X + Y * Y + Z * Z));
        }

        public static Vector Subtract(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector Cross(Vector v1, Vector v2)
        {
            return new Vector(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
        }

        public static Vector Normalize(Vector v)
        {
            return new Vector((float)(v.X / v.Length()), (float)(v.Y / v.Length()), (float)(v.Z / v.Length())); ;
        }

        public static float Dot(Vector v1, Vector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static Vector TransformCoordinate(Vector coord, Matrix transMat)
        {
            return new Vector(transMat.M11 * coord.X + transMat.M12 * coord.Y + transMat.M13 * coord.Z + transMat.M14 * 1,
                              transMat.M21 * coord.X + transMat.M22 * coord.Y + transMat.M23 * coord.Z + transMat.M24 * 1,
                              transMat.M31 * coord.X + transMat.M32 * coord.Y + transMat.M33 * coord.Z + transMat.M34 * 1);
        }

        //internal static Vector UnitY()
        //{
        //    Vector v = new Vector();
        //    v.X = 0;
        //    v.Y = 1;
        //    v.Z = 0;
        //    return v;
        //}
    }
}
