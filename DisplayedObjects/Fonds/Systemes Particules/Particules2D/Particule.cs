using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D
{
    public class Particule2D : IComparable
    {
        public bool active = false;
        public double debutVie ;
        public double finVie ;
        public float x, y;
        public float taille;
        public float vx, vy;
        public int textureIndex = 0;
        public float[] _couleur = new float[4];
        public bool _couleurIndividuelle = false;

        public float alpha
        {
            get
            {
                return _couleur[3];
            }

            set
            {
                _couleur[3] = value;
            }
        }

        public static double tempsEnMillisecondes( DateTime t )
        {
            return t.Subtract(Temps.BASEDATE).TotalMilliseconds;
        }

        int IComparable.CompareTo(Object o)
        {
            if (o is Particule2D)
            {
                Particule2D O2 = (Particule2D)o;
                if (active && !O2.active) return -1;
                if (!active && O2.active) return 1;

                if (debutVie < O2.debutVie) return -1;
                if (debutVie > O2.debutVie) return 1;
            }
            return 0;
        }
    }
}
