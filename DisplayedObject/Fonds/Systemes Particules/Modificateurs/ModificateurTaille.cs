using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Systeme_Particules.Modificateurs
{
    class ModificateurTaille : Modificateur
    {
        private float _dTaille;
        public ModificateurTaille(float dTaille)
        {
            _dTaille = dTaille;
        }

        public override void Applique(SystemeParticules s, Temps maintenant)
        {
            float dTaille = (_dTaille * maintenant._intervalle);
            int NbParticules = s._nbParticules;
            for (int i = 0; i < NbParticules; i++)
                if (s._particules[i].active)
                    s._particules[i].taille += dTaille;
        }
    }
}
