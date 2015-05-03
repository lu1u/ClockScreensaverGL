using System;
using System.Drawing;

namespace ClockScreenSaverGL.Fonds.Ete
{
    class Herbe
    {
        const float ratioSegments = 0.75f;
        const float largeurInitiale =10;
        const int NbSegments = 8;

        float _x, _y;
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
        public Herbe( float x, float y, float longueurSegment )
        {
            if (_pens == null)
                InitPens();
            _x = x;
            _y = y;

            _longueurSegments = new float[NbSegments];
            _anglesSegments = new double[NbSegments];

            double courbureHerbe = DisplayedObject.FloatRandom((float)-Math.PI, (float)Math.PI) / 20.0;
            float X = x;
            float Y = y;
            double angle = Math.PI;
            for ( int i = 0; i < NbSegments; i++)
            {
                _longueurSegments[i] = longueurSegment;
                _anglesSegments[i] = angle;
                longueurSegment *= ratioSegments;
                angle += courbureHerbe;
            }
        }

        private void InitPens()
        {
            _pens = new Pen[NbSegments];
            float l = largeurInitiale ;
            for ( int i = 0; i < NbSegments; i++)
            {
                _pens[i] = new Pen(Color.Black, l);
                l *= ratioSegments;
            }
        }


        public void Affiche( Graphics g, double vent)
        {
            float X = _x;
            float Y = _y;
            float largeur = largeurInitiale;

            float XX, YY;
            for ( int i = 1; i < NbSegments; i++)
            {
                XX = X + _longueurSegments[i] * (float)Math.Sin(_anglesSegments[i]+vent*i);
                YY = Y + _longueurSegments[i] * (float)Math.Cos(_anglesSegments[i]+vent*i);

                g.DrawLine(_pens[i], X, Y, XX, YY);

                X = XX;
                Y = YY;
                largeur *= ratioSegments;
            }
        }

        internal void Deplace(float vent)
        {
            for (int i = 0; i < NbSegments; i++)
                _anglesSegments[i] += vent;
        }
    }
}
