using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObject.Fonds.Printemps
{
    public class Vector3
    {
        public static readonly Vector3 Zero = new Vector3(0, 0 ,0);
        public float X;
        public float Y;
        public float Z;
        public Vector3(float x, float y, float z  )
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public void Normalize()
        {
            float l = Length();
            X /= l;
            Y /= l;
            Z /= l;
        }
        
        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z +- v2.Z);
        }

        public static Vector3 operator *(Vector3 v1, float m)
        {
            return new Vector3(v1.X * m, v1.Y * m, v1.Z * m);
        }

        public static float operator *(Vector3 v1, Vector3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static Vector3 operator /(Vector3 v1, float m)
        {
            return new Vector3(v1.X / m, v1.Y / m, v1.Z / m);
        }
        
        public static float Distance(Vector3 v1, Vector3 v2)
        {
            return (float)Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2* + Math.Pow(v1.Z - v2.Z, 2)));
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z*Z);
        }
        
        public PointF Point()
        {
            return new PointF( X, Y) ;
        }
    }
}
