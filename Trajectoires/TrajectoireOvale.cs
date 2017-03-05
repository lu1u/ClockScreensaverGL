using System;
using System.Drawing;

namespace ClockScreenSaverGL.Trajectoires
{
    class TrajectoireOvale : Trajectoire
    {
        const float PI_M_2 = (float)(Math.PI * 2.0);
        const float PI_SUR_2 = (float)(Math.PI / 2.0);

        private float _centreX, _centreY, _rayonX, _rayonY, _vitesseAngulaire, _angle;

        public TrajectoireOvale( float CentreX, float CentreY, float rayonX, float rayonY, float vitesseAngulaire, float angleInitial )
        {
            _centreX = CentreX;
            _centreY = CentreY;
            _rayonX = rayonX;
            _rayonY = rayonY;
            _vitesseAngulaire = vitesseAngulaire;
           _angle = angleInitial;
            CalculePosition();
        }

        private void CalculePosition()
        {
            _Px = (float)(_centreX + _rayonX * Math.Sin(_angle));
            _Py = (float)(_centreY + _rayonY * Math.Cos(_angle));

            _Vx = (float)(_vitesseAngulaire * _rayonX * Math.Sin(_angle + PI_SUR_2));
            _Vy = (float)(_vitesseAngulaire * _rayonX * Math.Cos(_angle + PI_SUR_2));
        }

        public override void Avance(RectangleF Bounds, SizeF Taille, Temps maintenant)
        {
            _angle += _vitesseAngulaire * maintenant.intervalleDepuisDerniereFrame;
            CalculePosition();
        }

        public override void Avance(Rectangle Bounds, SizeF Taille, Temps maintenant)
        {
            _angle += _vitesseAngulaire * maintenant.intervalleDepuisDerniereFrame;
            CalculePosition();
        }
    }
}
