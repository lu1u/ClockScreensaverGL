using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Systeme_Particules.Modificateurs
{
    class ModificateurVitesseLineaire : Modificateur
    {
        private DateTime _derniere = DateTime.Now;

        public override void Applique(SystemeParticules s, Temps maintenant)
        {
            for (int i = 0; i < s._nbParticules; i++)
                if (s._particules[i].active)
                {
                    s._particules[i].x += s._particules[i].vx * maintenant._intervalle;
                    s._particules[i].y += s._particules[i].vy * maintenant._intervalle;
                }
        }
    }
}
