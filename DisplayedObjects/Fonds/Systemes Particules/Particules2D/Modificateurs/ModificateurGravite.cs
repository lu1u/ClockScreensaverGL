using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs
{
    class ModificateurGravite: Modificateur
    {
        private float _gX, _gY;
        public ModificateurGravite( float gX, float gY )
        {
            _gX = gX;
            _gY = gY;
        }

        public override void Applique(SystemeParticules2D s, Temps maintenant)
        {
            float dX = _gX * maintenant.intervalleDepuisDerniereFrame;
            float dY = _gY * maintenant.intervalleDepuisDerniereFrame;
            int NbParticules = s._nbParticules;
            for (int i = 0; i < NbParticules; i++)
                if (s._particules[i].active)
                {
                    s._particules[i].vx += dX;
                    s._particules[i].vy += dY ;
                }

        }
    }
}
