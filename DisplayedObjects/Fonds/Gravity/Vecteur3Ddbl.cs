using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity
{
    public class Vecteur3Ddbl
    {
        public double x, y, z;
        public static readonly Vecteur3Ddbl NULL = new Vecteur3Ddbl(0, 0, 0);
        public static readonly Vecteur3Ddbl UN = new Vecteur3Ddbl(1, 1, 1);
        public static readonly Vecteur3Ddbl Z = new Vecteur3Ddbl(0, 0, 1);
        public static readonly Vecteur3Ddbl MOINS_Z = new Vecteur3Ddbl(0, 0, -1);
        public static readonly Vecteur3Ddbl Y = new Vecteur3Ddbl(0, 1, 0);
        public static readonly Vecteur3Ddbl MOINS_Y = new Vecteur3Ddbl(0, -1, 0);
        public static readonly Vecteur3Ddbl X = new Vecteur3Ddbl(1, 0, 0);
        public static readonly Vecteur3Ddbl MOINS_X = new Vecteur3Ddbl(-1, 0, 0);

        public Vecteur3Ddbl()
        {
        }
        public Vecteur3Ddbl(double X, double Y, double Z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public Vecteur3Ddbl(Vecteur3Ddbl v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
        public Vecteur3Ddbl(double v)
        {
            x = v;
            y = v;
            z = v;
        }

        public double Longueur()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public void Normalize()
        {
            double n = Longueur();
            x /= n;
            y /= n;
            z /= n;
        }

        public double[] tabd
        {
            get
            {
                double[] f = new double[3];
                f[0] = x;
                f[1] = y;
                f[2] = z;
                return f;
            }
        }
        public void Vertex(OpenGL gl)
        {
            gl.Vertex(x, y, z);
        }

        public void Normal(OpenGL gl)
        {
            gl.Normal(x, y, z);
        }
        static public Vecteur3Ddbl operator *(double f, Vecteur3Ddbl v)     //produit par un réel
        {
            Vecteur3Ddbl z = v;
            z.multiplier_par(f);
            return (z);
        }

        public static Vecteur3Ddbl operator *(Vecteur3Ddbl v, double f)     //le prod par un double est commutatif !!!
        { return (f * v); }    //je l'ai déjà défini dans l'autre sens, autant s'en servir !

        public static Vecteur3Ddbl operator /(Vecteur3Ddbl v, double f)
        { return (v * (1 / f)); }

        static public double operator *(Vecteur3Ddbl v, Vecteur3Ddbl w)     //produit scalaire
        { return v.prodscal(w); }

        static public Vecteur3Ddbl operator +(Vecteur3Ddbl v, Vecteur3Ddbl w)     //somme vectorielle
        {
            return new Vecteur3Ddbl(v.x + w.x, v.y + w.y, v.z + w.z);
        }

        static public Vecteur3Ddbl operator -(Vecteur3Ddbl v, Vecteur3Ddbl w)     //différence vectorielle
        { return new Vecteur3Ddbl(v.x - w.x, v.y - w.y, v.z - w.z); }

        static public Vecteur3Ddbl operator -(Vecteur3Ddbl v)     //negatif
        { return new Vecteur3Ddbl(-v.x, -v.y, -v.z); }

        static public Vecteur3Ddbl operator ^(Vecteur3Ddbl v, Vecteur3Ddbl w)     //produit vectoriel
        {
            Vecteur3Ddbl z = new Vecteur3Ddbl(
                v.y * w.z - w.y * v.z,
                v.z * w.x - w.z * v.x,
                v.x * w.y - w.x * v.y
            );
            return z;
        }
        public Vecteur3Ddbl Cross(Vecteur3Ddbl w)     //produit vectoriel
        {
            Vecteur3Ddbl z = new Vecteur3Ddbl((this.y * w.z) - (w.y * this.z),
                (this.z * w.x) - (w.z * x),
                (x * w.y) - (w.x * y)
            );
            return z;
        }
        public void multiplier_par(double a) { x = a * x; y = a * y; z = a * z; }
        public void diviser_par(double a) { x = x / a; y = y / a; z = z / a; }
        public double prodscal(Vecteur3Ddbl v) { return (x * v.x + y * v.y + z * v.z); }
        public void additionner(double a) { x = a + x; y = a + y; z = a + z; }
        public void additionner(double a, double b, double c) { x = a + x; y = b + y; z = c + z; }
        public void additionner(Vecteur3Ddbl a) { x = x + a.x; y = y + a.y; z = z + a.z; }

        public void soustraire(Vecteur3Ddbl a) { x = x - a.x; y = y - a.y; z = z - a.z; }

        static double DEG_TO_RAD(double a) { return a * (double)Math.PI / 360.0f; }
        public void RotateX(double AngleDegres)
        {
            double Angle = DEG_TO_RAD(AngleDegres);
            y = (double)((Math.Cos(Angle) * y) - (Math.Sin(Angle) * z));
            z = (double)((Math.Sin(Angle) * y) + (Math.Cos(Angle) * z));
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Rotaton du vecteur autour de l'axe des Y
        // ENTREES:	Angle en degres
        ///////////////////////////////////////////////////////////////////////////////
        public void RotateY(double AngleDegres)
        {
            double Angle = DEG_TO_RAD(AngleDegres);
            x = (double)((Math.Cos(Angle) * x) + (Math.Sin(Angle) * z));
            z = (double)(-(Math.Sin(Angle) * x) + (Math.Cos(Angle) * z));
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Rotaton du vecteur autour de l'axe des Z
        // ENTREES:	Angle en degres
        ///////////////////////////////////////////////////////////////////////////////
        public void RotateZ(double AngleDegres)
        {
            double Angle = DEG_TO_RAD(AngleDegres);
            x = (double)((Math.Cos(Angle) * x) - (Math.Sin(Angle) * y));
            y = (double)((Math.Sin(Angle) * x) + (Math.Cos(Angle) * y));
        }
    }
}
