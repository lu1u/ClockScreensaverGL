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

        public Vecteur3D()
        {
        }
        public Vecteur3D(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }


        float norme()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public void Normalize()
        {
            float n = norme();
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
        static public Vecteur3D operator *(Vecteur3D v, float f)     //le prod par un float est commutatif !!!
        { return (f * v); }    //je l'ai déjà défini dans l'autre sens, autant s'en servir !

        static public Vecteur3D operator /(Vecteur3D v, float f)
        { return (v * (1 / f)); }

        static public float operator *(Vecteur3D v, Vecteur3D w)     //produit scalaire
        { return v.prodscal(w); }

        static public Vecteur3D operator +(Vecteur3D v, Vecteur3D w)     //somme vectorielle
        {
            Vecteur3D z = v;
            v.additionner(w);
            return (z);
        }

        static public Vecteur3D operator -(Vecteur3D v, Vecteur3D w)     //différence vectorielle
        { return (v + ((-1) * w)); }

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
        void multiplier_par(float a) { x = a * x; y = a * y; z = a * z; }
        float prodscal(Vecteur3D v) { return (x * v.x + y * v.y + z * v.z); }
        void additionner(float a)
        { x = a + x; y = a + y; z = a + z; }
        void additionner(float a, float b, float c)
        { x = a + x; y = b + y; z = c + z; }
        public void additionner(Vecteur3D a)
        { x = x + a.x; y = y + a.y; z = z + a.z; }
    }
}
