/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 17/01/2015
 * Heure: 13:04
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using SharpGL;
using System;


namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD
{
    /// <summary>
    /// Description of Vecteur3D.
    /// </summary>
    public class Vecteur3D
    {
        public float x, y, z;
        public static readonly Vecteur3D NULL = new Vecteur3D(0, 0, 0);
        public static readonly Vecteur3D UN = new Vecteur3D(1, 1, 1);
        public static readonly Vecteur3D Z = new Vecteur3D(0, 0, 1);
        public static readonly Vecteur3D MOINS_Z = new Vecteur3D(0, 0, -1);
        public static readonly Vecteur3D Y = new Vecteur3D(0, 1, 0);
        public static readonly Vecteur3D MOINS_Y = new Vecteur3D(0, -1, 0);
        public static readonly Vecteur3D X = new Vecteur3D(1, 0, 0);
        public static readonly Vecteur3D MOINS_X = new Vecteur3D(-1, 0, 0);

        public Vecteur3D()
        {
        }
        public Vecteur3D(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public Vecteur3D(Vecteur3D v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public float Longueur()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public void Normalize()
        {
            float n = Longueur();
            x /= n;
            y /= n;
            z /= n;
        }

        public float[] tabf
        {
            get
            {
                float[] f = new float[3];
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
        static public Vecteur3D operator *(float f, Vecteur3D v)     //produit par un réel
        {
            Vecteur3D z = v;
            z.multiplier_par(f);
            return (z);
        }

        public static Vecteur3D operator *(Vecteur3D v, float f)     //le prod par un float est commutatif !!!
        { return (f * v); }    //je l'ai déjà défini dans l'autre sens, autant s'en servir !

        public static Vecteur3D operator /(Vecteur3D v, float f)
        { return (v * (1 / f)); }

        static public float operator *(Vecteur3D v, Vecteur3D w)     //produit scalaire
        { return v.prodscal(w); }

        static public Vecteur3D operator +(Vecteur3D v, Vecteur3D w)     //somme vectorielle
        {
            return new Vecteur3D(v.x+w.x, v.y+w.y, v.z+w.z) ;
        }

        static public Vecteur3D operator -(Vecteur3D v, Vecteur3D w)     //différence vectorielle
        { return new Vecteur3D(v.x - w.x, v.y - w.y, v.z - w.z); }

        static public Vecteur3D operator -(Vecteur3D v)     //negatif
        { return new Vecteur3D(-v.x, -v.y, -v.z); }

        static public Vecteur3D operator ^(Vecteur3D v, Vecteur3D w)     //produit vectoriel
        {
            Vecteur3D z = new Vecteur3D(
                v.y * w.z - w.y * v.z,
                v.z * w.x - w.z * v.x,
                v.x * w.y - w.x * v.y
            );
            return z;
        }
        public Vecteur3D Cross(Vecteur3D w)     //produit vectoriel
        {
            Vecteur3D z = new Vecteur3D((this.y * w.z) - (w.y * this.z),
                (this.z * w.x) - (w.z * x),
                (x * w.y) - (w.x * y)
            );
            return z;
        }
        public void multiplier_par(float a) { x = a * x; y = a * y; z = a * z; }
        public void diviser_par(float a) { x = x/a; y = y/a; z = z/a; }
        public float prodscal(Vecteur3D v) { return (x * v.x + y * v.y + z * v.z); }
        public void additionner(float a) { x = a + x; y = a + y; z = a + z; }
        public void additionner(float a, float b, float c) { x = a + x; y = b + y; z = c + z; }
        public void additionner(Vecteur3D a) { x = x + a.x; y = y + a.y; z = z + a.z; }

        public void soustraire(Vecteur3D a) { x = x - a.x; y = y - a.y; z = z - a.z; }

        static float DEG_TO_RAD( float a) { return a * (float)Math.PI / 360.0f;  }
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
    }
}
