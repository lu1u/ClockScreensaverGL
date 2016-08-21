using System;

namespace ClockScreenSaverGL.DisplayedObject.Fonds.Gravitation
{
    public class CVecteur3D
    {
        public static readonly CVecteur3D Zero = new CVecteur3D(0, 0, 0);
        public static readonly CVecteur3D X = new CVecteur3D(1, 0, 0);
        public static readonly CVecteur3D MoinsX = new CVecteur3D(-1, 0, 0);
        public static readonly CVecteur3D Y = new CVecteur3D(0, 1, 0);
        public static readonly CVecteur3D MoinsY = new CVecteur3D(0, -1, 0);
        public static readonly CVecteur3D Z = new CVecteur3D(0, 0, 1);
        public static readonly CVecteur3D MoinsZ = new CVecteur3D(0, 0, -1);

        private float x, y, z;

        public CVecteur3D()
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
        }

        public CVecteur3D(CVecteur3D V)
        {
            x = V.x;
            y = V.y;
            z = V.z;
        }

        public CVecteur3D(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public void Set(CVecteur3D V)
        {
            x = V.x;
            y = V.y;
            z = V.z;
        }
        void Set(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        public void AngleAleatoire(float Rayon)
        {
            Random r = new Random();
            float Phi = (float)(r.Next(6283) / 1000.0f);
            float Theta = (float)(r.Next(6283) / 1000.0f);

            x = Rayon * (float)(Math.Cos(Theta) * Math.Cos(Phi));
            y = Rayon * (float)(Math.Cos(Theta) * Math.Sin(Phi));
            z = Rayon * (float)(Math.Sin(Theta));
        }

        public static float CalculeAngle(float x, float y)
        {
            if (x == 0)
            {
                if (y < 0)
                    return 180.0f;
                else
                    return 0.0f;
            }
            else
                return (float)(Math.Atan2(x, -y) * 180.0f / Math.PI) + 180.0f;
        }

        public static double DEG_TO_RAD(float deg)
        {
            return deg * 180.0f / Math.PI;
        }
        ///////////////////////////////////////////////////////////////////////////////
        // Rotaton du vecteur autour de l'axe des X
        // ENTREES:	Angle en degres
        ///////////////////////////////////////////////////////////////////////////////
        public void RotateX(float AngleDegres)
        {
            double Angle = DEG_TO_RAD(AngleDegres);
            y = (float)((Math.Cos(Angle) * y) - (Math.Sin(Angle) * z));
            z = (float)((Math.Sin(Angle) * y) + (Math.Cos(Angle) * z));
        }


        ///////////////////////////////////////////////////////////////////////////////
        // Rotaton du vecteur autour de l'axe des Y
        // ENTREES:	Angle en degres
        ///////////////////////////////////////////////////////////////////////////////
        public void RotateY(float AngleDegres)
        {
            double Angle = DEG_TO_RAD(AngleDegres);
            x = (float)((Math.Cos(Angle) * x) + (Math.Sin(Angle) * z));
            z = (float)(-(Math.Sin(Angle) * x) + (Math.Cos(Angle) * z));
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Rotaton du vecteur autour de l'axe des Z
        // ENTREES:	Angle en degres
        ///////////////////////////////////////////////////////////////////////////////
        public void RotateZ(float AngleDegres)
        {
            double Angle = DEG_TO_RAD(AngleDegres);
            x = (float)((Math.Cos(Angle) * x) - (Math.Sin(Angle) * y));
            y = (float)((Math.Sin(Angle) * x) + (Math.Cos(Angle) * y));
        }
        // Operations sur le vecteur
        public void Nulle() { x = 0; y = 0; z = 0; }
        public void Plus(CVecteur3D V) { x += V.x; y += V.y; z += V.z; }
        public void Moins(CVecteur3D V) { x -= V.x; y -= V.y; z -= V.z; }
        public void Multiplie(CVecteur3D V) { x *= V.x; y *= V.y; z *= V.z; }
        public void Divise(CVecteur3D V) { x /= V.x; y /= V.y; z /= V.z; }

        public void Plus(float _x, float _y, float _z) { x += _x; y += _y; z += _z; }
        public void Moins(float _x, float _y, float _z) { x -= _x; y -= _y; z -= _z; }
        public void Multiplie(float _x, float _y, float _z) { x *= _x; y *= _y; z *= _z; }

        public void Plus(float P) { x += P; y += P; z += P; }
        public void Moins(float P) { x -= P; y -= P; z -= P; }
        public void Multiplie(float P) { x *= P; y *= P; z *= P; }
        public void Divise(float P) { x /= P; y /= P; z /= P; }
        public float Longueur() { return		(float)Math.Sqrt(	  (x* x)  + (y* y ) + (z* z) ) ; }
    static public CVecteur3D operator -(CVecteur3D v, CVecteur3D w)
        {
            return new CVecteur3D(v.x - w.x, v.y - w.y, v.z - w.z);
        }
    }
}
