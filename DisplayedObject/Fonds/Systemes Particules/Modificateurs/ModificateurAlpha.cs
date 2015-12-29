using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Systeme_Particules.Modificateurs
{
    class ModificateurAlpha: Modificateur
    {
        private DateTime _derniere = DateTime.Now;
        private float _dAlpha;
        public ModificateurAlpha(float dAlpha)
        {
            _dAlpha = dAlpha;
        }

        public override void Applique(SystemeParticules s, Temps maintenant )
        {
            float dAlpha = (_dAlpha) * maintenant._intervalle;
            int NbParticules = s._nbParticules;
            for (int i = 0; i < NbParticules; i++)
                if (s._particules[i].active)
                {
                    s._particules[i].alpha -= dAlpha;
                }
        }
    }
}
