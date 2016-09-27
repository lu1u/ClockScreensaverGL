using SharpGL;
using System;
using System.Drawing;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Saisons.Ete
{
    class Herbe
    {
        const float ratioSegments = 0.8f;
        const float largeurInitiale = 15;
        const int NbSegments = 16;

        readonly float _x, _y;
        readonly float _raideur;
        float[] _longueurSegments;
        double[] _anglesSegments;
        static Pen[] _pens;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="NbSegments"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="longueurSegment"></param>
        public Herbe(float x, float y, float longueurSegment, float raideur)
        {
            if (_pens == null)
                InitPens();
            _x = x;
            _y = y;
            _raideur = raideur;
            _longueurSegments = new float[NbSegments];
            _anglesSegments = new double[NbSegments];

            double courbureHerbe = DisplayedObject.FloatRandom((float)-Math.PI, (float)Math.PI) / 20.0;
            float X = x;
            float Y = y;
            double angle = Math.PI;
            for (int i = 0; i < NbSegments; i++)
            {
                _longueurSegments[i] = longueurSegment;
                _anglesSegments[i] = angle;
                longueurSegment *= ratioSegments;
                angle += courbureHerbe;
            }
        }

        private static void InitPens()
        {
            _pens = new Pen[NbSegments];
            float l = largeurInitiale;
            for (int i = 0; i < NbSegments; i++)
            {
                _pens[i] = new Pen(Color.Black, l);
                l *= ratioSegments;
            }
        }

        public void Affiche(OpenGL gl, double vent)
        {
            float X = _x;
            float Y = _y;
            float largeur = largeurInitiale * 0.75f;

            vent *= _raideur;
            gl.PushMatrix();
            gl.Translate(_x, _y, 0);

            // trace les elements du brin d'herbe
            for (int i = 1; i < NbSegments; i++)
            {
                gl.Begin(OpenGL.GL_QUADS);
                gl.Vertex(-largeur, 0);
                gl.Vertex(-largeur, -_longueurSegments[i]);
                gl.Vertex(largeur, -_longueurSegments[i]);
                gl.Vertex(largeur, 0);
                gl.End();
                largeur = largeur * ratioSegments;

                gl.Translate(0, -_longueurSegments[i], 0);
                gl.Rotate(0, 0, (float)(_anglesSegments[i] + (vent * i)));
            }

            gl.PopMatrix();
        }
        internal void Deplace(float vent)
        {
            for (int i = 0; i < NbSegments; i++)
                _anglesSegments[i] += vent;
        }
    }
}
