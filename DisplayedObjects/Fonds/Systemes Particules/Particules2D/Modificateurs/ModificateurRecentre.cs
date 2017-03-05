using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs
{
    class ModificateurRecentre: Modificateur
    {
        private DateTime _derniere = DateTime.Now;
        private float _vitesse;
        public ModificateurRecentre(float vitesse)
        {
            _vitesse = vitesse;
        }

        public override void Applique(SystemeParticules2D s, Temps maintenant )
        {
            int indice = 0;
            float tailleMax = -1;
            for (int i = 0; i < s._nbParticules; i++)
                if (s._particules[i].active)
                    if (s._particules[i].taille > tailleMax)
                    {
                        indice = i;
                        tailleMax = s._particules[i].taille;
                    }


            // Decaler toutes les particules pour attirer la plus grosse au centre
            float decalageX = s._particules[indice].x * _vitesse * maintenant.intervalleDepuisDerniereFrame;
            float decalageY = s._particules[indice].y * _vitesse * maintenant.intervalleDepuisDerniereFrame;
            float decalageVX = s._particules[indice].vx * _vitesse * maintenant.intervalleDepuisDerniereFrame;
            float decalageVY = s._particules[indice].vy * _vitesse * maintenant.intervalleDepuisDerniereFrame;

            for (int i = 0; i < s._nbParticules; i++)
                if (s._particules[i].active)
                {
                    s._particules[i].x -= decalageX;
                    s._particules[i].y -= decalageY;
                    s._particules[i].vx -= decalageVX;
                    s._particules[i].vy -= decalageVY;
                }
        }
    }
}
