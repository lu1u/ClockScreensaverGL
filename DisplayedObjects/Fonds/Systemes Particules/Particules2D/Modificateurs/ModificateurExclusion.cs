using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs
{
    class ModificateurExclusion : Modificateur
    {
        public enum Exclusions
        {
            EXCLURE_AU_DESSUS = 1,
            EXCLURE_EN_DESSOUS = 2,
            EXCLURE_A_DROITE = 4,
            EXCLURE_A_GAUCHE = 8,
            EXCLURE_DROITE_GAUCHE = 12,
            EXCLURE_HAUT_BAS = 3,
            EXCLURE_TOUT = 15
        }

        private Exclusions exclusions; // Une combinaison des constantes EXCLURE_*
        private float _MinX, _MinY, _MaxX, _MaxY;

        public ModificateurExclusion(float MinX, float MinY, float MaxX, float MaxY, Exclusions excl)
        {
            _MinX = MinX;
            _MinY = MinY;
            _MaxX = MaxX;
            _MaxY = MaxY;
            exclusions = excl;

        }
        public override void Applique(SystemeParticules2D s, Temps m)
        {
            int NbParticules = s._nbParticules;
            for (int i = 0; i < NbParticules; i++)
                if (s._particules[i].active)
                {
                    if (((exclusions & Exclusions.EXCLURE_AU_DESSUS) != 0))
                    {
                        // Exclure la particule si elle est au dessus du rectangle
                        if (s._particules[i].y > _MaxY)
                            s._particules[i].active = false;
                    }

                    if (((exclusions & Exclusions.EXCLURE_EN_DESSOUS) != 0))
                    {
                        // Exclure la particule si elle est en dessous du rectangle
                        if (s._particules[i].y < _MinX)
                            s._particules[i].active = false;
                    }

                    if (((exclusions & Exclusions.EXCLURE_A_DROITE) != 0))
                    {
                        // Exclure la particule si elle est a droite du rectangle
                        if (s._particules[i].x > _MaxX)
                            s._particules[i].active = false;
                    }

                    if (((exclusions & Exclusions.EXCLURE_A_GAUCHE) != 0))
                    {
                        // Exclure la particule si elle est a gauche du rectangle
                        if (s._particules[i].x < _MinX)
                            s._particules[i].active = false;
                    }
                }
        }
    }
}
