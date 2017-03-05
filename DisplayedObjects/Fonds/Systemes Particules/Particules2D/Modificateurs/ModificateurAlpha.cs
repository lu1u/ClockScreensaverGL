using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs
{
    class ModificateurAlpha: Modificateur
    {
        const float SEUIL = 0.001f;
        private DateTime _derniere = DateTime.Now;
        private float _dAlpha;
        private bool _eliminerSiSeuil;
        public ModificateurAlpha(float dAlpha, bool eliminerSiSeuil = false)
        {
            _dAlpha = dAlpha;
            _eliminerSiSeuil = eliminerSiSeuil;
        }

        public override void Applique(SystemeParticules2D s, Temps maintenant )
        {
            float dAlpha = (_dAlpha) * maintenant.intervalleDepuisDerniereFrame;
            int NbParticules = s._nbParticules;
            for (int i = 0; i < NbParticules; i++)
                if (s._particules[i].active)
                {
                    s._particules[i].alpha -= dAlpha;
                    if (_eliminerSiSeuil)
                        if (s._particules[i].alpha < SEUIL)
                        {
                            s._particules[i].active = false;
                            s._trier = true;
                        }
                }
        }
    }
}
