using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs
{
    class ModificateurLife : Modificateur
    {
        
        public override void Applique(SystemeParticules2D s, Temps maintenant)
        {
            int NbParticules = s._nbParticules;
            bool existeApres = false;
            for (int i = NbParticules-1; i >= 0; i--)
                if (s._particules[i].active)
                {
                    if (maintenant.totalMilliSecondes > s._particules[i].finVie )
                    {
                        s._particules[i].active = false ;
                        s.Trier = true;
                        if (!existeApres)
                            s._nbParticules = i;
                    }
                    else
                        existeApres = true;
                }            
        }
    }
}
