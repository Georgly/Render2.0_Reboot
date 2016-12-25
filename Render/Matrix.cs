using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    public struct Matrix
    {
        public float M11;

        public float M12;

        public float M13;

        public float M14;

        public float M21;

        public float M22;

        public float M23;

        public float M24;

        public float M31;

        public float M32;

        public float M33;

        public float M34;

        public float M41;

        public float M42;

        public float M43;

        public float M44;

        internal static Matrix LookAtLH(Vector position, Vector target, Vector unitY)
        {
            Vector rightn, upn, vec;
            Matrix pout = new Matrix();

            vec = Vector.Normalize(Vector.Subtract(target, position));
            rightn = Vector.Normalize(Vector.Cross(unitY, vec));
            upn = Vector.Normalize(Vector.Cross(vec, Vector.Cross(unitY, vec)));
            pout.M11 = rightn.X;
            pout.M21 = rightn.Y;
            pout.M31 = rightn.Z;
            pout.M41 = -Vector.Dot(rightn, position);
            pout.M12 = upn.X;
            pout.M22 = upn.Y;
            pout.M32 = upn.Z;
            pout.M42 = -Vector.Dot(upn, position);
            pout.M13 = vec.X;
            pout.M23 = vec.Y;
            pout.M33 = vec.Z;
            pout.M43 = -Vector.Dot(vec, position);
            pout.M44 = 1.0f;
            return pout;
        }

        internal static Matrix PerspectiveFovRH(float v1, float p, float v2, float v3)
        {
            float yScale = 1 / (float)Math.Tan(/*(Math.PI * v1 / 180)*/v1 / 2);
            float xScale = yScale / p;
            Matrix result = new Matrix();
            //first line
            result.M11 = xScale;
            //second line
            result.M22 = yScale;
            //third line
            result.M33 = v3 / (v2 - v3);
            result.M34 = -1;
            //fourth line
            result.M43 = v2 * v3 / (v2 - v3);
            return result;
        }

        internal static Matrix RotationYawPitchRoll(double roll, double pitch, double yaw)
        {
            //throw new NotImplementedException();
            Matrix m = new Matrix();
            m.M11 = (float)((Math.Cos(roll) * Math.Cos(yaw)) + (Math.Sin(roll) * Math.Sin(pitch) * Math.Sin(yaw)));
	        m.M12 = (float)(Math.Sin(roll) * Math.Cos(pitch));
	        m.M13 = (float)((Math.Cos(roll) * (-Math.Sin(yaw))) + (Math.Sin(roll) * Math.Sin(pitch) * Math.Cos(yaw)));
	        m.M21 = (float)((-Math.Sin(roll) * Math.Cos(yaw)) + (Math.Cos(roll) * Math.Sin(pitch) * Math.Sin(yaw)));
	        m.M22 = (float)(Math.Cos(roll) * Math.Cos(pitch));
	        m.M23 = (float)((Math.Sin(roll) * Math.Sin(yaw)) + (Math.Cos(roll) * Math.Sin(pitch) * Math.Cos(yaw)));
            m.M31 = (float)(Math.Cos(pitch) * Math.Sin(yaw));
	        m.M32 = -(float)Math.Sin(pitch);
	        m.M33 = (float)(Math.Cos(pitch) * Math.Cos(yaw));
            m.M44 = 1;
            return m;
        }

        internal static Matrix Translation(Vector position)
        {
            Matrix result = new Matrix();
            //first line
            result.M11 = 1;
            result.M14 = position.X;
            //second line
            result.M22 = 1;
            result.M24 = position.Y;
            //third line
            result.M33 = 1;
            result.M34 = position.Z;
            //fourth line
            result.M44 = 1;
            return result;
        }

        public static Matrix operator *(Matrix left, Matrix right)
        {
            Matrix result = new Matrix();
            //first line
            result.M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41;
            result.M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42;
            result.M13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43;
            result.M14 = left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44;
            //second line
            result.M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41;
            result.M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42;
            result.M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43;
            result.M24 = left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44;
            //third line
            result.M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41;
            result.M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42;
            result.M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43;
            result.M34 = left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44;
            //fourth line
            result.M41 = left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41;
            result.M42 = left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42;
            result.M43 = left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43;
            result.M44 = left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44;
            return result;
        }

        public static Matrix Scale(float coef)
        {
            Matrix result = new Matrix();
            //first line
            result.M11 = coef;
            //second line
            result.M22 = coef;
            //third line
            result.M33 = coef;
            //fourth line
            result.M44 = 1;
            return result;
        }
    }
}
